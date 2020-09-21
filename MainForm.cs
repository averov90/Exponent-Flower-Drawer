using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ExponentFlowerDrawer {
    public partial class MainForm : Form {
        public MainForm() {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();
        }
        private void MainForm_Shown(object sender, EventArgs e) {
            cmd_log.SelectedIndex = 0;
        }
        private void cmd_log_DoubleClick(object sender, EventArgs e) {
            if (cmd_log.SelectedIndex != -1 && cmd_log.Items[cmd_log.SelectedIndex].ToString().StartsWith("> "))
                cmd.Text = cmd_log.Items[cmd_log.SelectedIndex].ToString().Remove(0, 2);
            else
                cmd_log.Items.AddRange(new string[] { "> ", "error: not a command" });
        }

        private void cmd_log_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                if (cmd_log.SelectedIndex != -1 && cmd_log.Items[cmd_log.SelectedIndex].ToString().StartsWith("> "))
                    cmd.Text = cmd_log.Items[cmd_log.SelectedIndex].ToString().Remove(0, 2);
                else
                    cmd_log.Items.AddRange(new string[] { "> ", "error: not a command" });
        }

        private void button1_Click(object sender, EventArgs e) {
            Cmd_Execute();
        }
        private void cmd_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                Cmd_Execute();
        }

        float exp_y_limit = 4, deform = 1, rotation = 0;
        byte petals_count = 8;
        double log_basis = Math.E;
        bool draw_connhideDots = true, draw_centerDot = false, draw_polygon = false, draw_innerCircle = false, rotation_absolute = false;

        Graphics gr;
        Pen pen; Brush brush;
        Bitmap image;
        bool regen = false, reinit = false, need_pinit = true;
        bool background_alpha = false, color_inverse = false;
        ushort image_size = 1024; //Square
        float pen_width_percent = 4;

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (cmd.ForeColor == Color.White) {
                if (MessageBox.Show("Why are you closing this program?\nBecause it has a black theme?\nBlack lives matter!", "black lives matter", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                    if (MessageBox.Show("\"Yes\" means you agree that black lives matter?", "black lives matter", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        MessageBox.Show("You are a racist!\nWhy don't you respect blacks?\nYou should value blacks as much as whites.\nThere is not a single world war on blacks.\nPerhaps they are even better than the whites.\nSo sorry that science is not yet able to darken the color of the skin.", "racist detected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                } else {
                    if (MessageBox.Show("\"No\" means that you are closing the program not because of a black theme?", "black lives matter", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        MessageBox.Show("You are a racist!\nWhy don't you respect blacks?\nYou should value blacks as much as whites.\nThere is not a single world war on blacks.\nPerhaps they are even better than the whites.\nSo sorry that science is not yet able to darken the color of the skin.", "racist detected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        bool RecogniseBool(string input, ref bool output) {

            if (bool.TryParse(input, out bool temp)) {
                output = temp;
                return true;
            }
            if (int.TryParse(input, out int temp1)) {
                output = temp1 != 0;
                return true;
            }
            return false;
        }

        private Bitmap GenSymmetricExponent(float pen_width, float pen_width_half, out double y_center_distance, out double radius) {
            float image_size_fake = image_size - pen_width - pen_width_half;

            double eratio = exp_y_limit / Math.Log(exp_y_limit + 1, log_basis),
                ebwidtHalf = image_size_fake / (2 * eratio + 2 / Math.Tan(Math.PI / petals_count)),
                ebheight = ebwidtHalf * eratio,
                printToE = exp_y_limit * (deform > 0 ? deform : -deform) / ebheight, //Can be replaced by (exp_y_limit / ebheight) if deform isn't need
                eToPrint = ebheight / (Math.Pow(log_basis, (ushort)ebwidtHalf * printToE) - 1); //Can be replaced by (ebheight / exp_y_limit) if deform isn't need

            radius = (image_size_fake - 2 * ebheight) / 2;

            if (radius < image_size_fake * 0.01 || radius > image_size_fake * 0.90) {
                y_center_distance = 0;
                return null;
            }

            ebheight += pen_width_half;
            Bitmap exponent = new Bitmap((int)Math.Round(ebwidtHalf + ebwidtHalf + pen_width), (int)Math.Round(ebheight + pen_width_half * 1.5), PixelFormat.Format32bppArgb);
            Graphics egr = Graphics.FromImage(exponent);
            egr.SmoothingMode = SmoothingMode.AntiAlias;
            egr.CompositingQuality = CompositingQuality.HighQuality;

            float dot_y = (float)ebheight,
                ebwidtHalf_offset;
            double ebwidtHalf_provider = deform > 0 ? ebwidtHalf : 0;

            egr.FillEllipse(brush, (float)ebwidtHalf_provider, dot_y - pen_width_half, pen_width, pen_width);
            for (ushort i = 1; i < ebwidtHalf; ++i) {
                ebwidtHalf_offset = (float)(ebwidtHalf_provider + i + pen_width_half);
                egr.DrawLine(pen, ebwidtHalf_offset - 1, dot_y, ebwidtHalf_offset,
                    dot_y = (float)(ebheight - (Math.Pow(log_basis, i * printToE) - 1) * eToPrint));
                egr.FillEllipse(brush, ebwidtHalf_offset - pen_width_half, dot_y - pen_width_half, pen_width, pen_width);
            }

            if (!(deform > 0)) exponent.RotateFlip(RotateFlipType.RotateNoneFlipY);

            Bitmap exponent_mirrored = (Bitmap)exponent.Clone();
            exponent_mirrored.RotateFlip(RotateFlipType.RotateNoneFlipX);
            egr.DrawImageUnscaled(exponent_mirrored, 0, 0);
            exponent_mirrored.Dispose();

            y_center_distance = (ebheight - dot_y) / 2;

            return exponent;
        }

        private bool Gen() {
            float pen_width = image_size * pen_width_percent / 400, pen_width_half = pen_width / 2;

            Bitmap exponent = GenSymmetricExponent(pen_width, pen_width_half, out double y_offset, out double radius);

            if (exponent == null) return false;

            ushort exponentRotator_size = (ushort)Math.Ceiling(Math.Sqrt(exponent.Width * exponent.Width + exponent.Height * exponent.Height));
            Point exponentRotator_position = new Point((int)Math.Round((exponentRotator_size - exponent.Width) / 2.0), (int)Math.Round((exponentRotator_size - exponent.Height) / 2.0));
            float exponentRotator_size_half = exponentRotator_size / 2.0F;

            Bitmap exponentRotator = new Bitmap(exponentRotator_size, exponentRotator_size, PixelFormat.Format32bppArgb);
            Graphics gfx = Graphics.FromImage(exponentRotator);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.CompositingQuality = CompositingQuality.HighQuality;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            float angle_degrees = 360.0F / petals_count, pen_width_quarter = pen_width_half / 2;
            double stick = radius + y_offset, image_size_half = Math.Round(image_size / 2.0), image_size_half_expRotator = image_size_half - exponentRotator_size_half, image_size_half_dot = image_size_half - pen_width_half,
                angle_radians = 2 * Math.PI / petals_count, angle_radians_dot = angle_radians / 2,
                dot_radius = radius / Math.Cos(Math.PI / petals_count) - pen_width_quarter;

            if (background_alpha) gr.Clear(Color.Transparent);
            else {
                if (color_inverse) gr.Clear(Color.Black);
                else gr.Clear(Color.White);
            }

            if (draw_centerDot) gr.FillEllipse(brush, (float)(image_size_half - pen_width_half), (float)(image_size_half - pen_width_half), pen_width, pen_width);
            if (draw_innerCircle) gr.DrawEllipse(pen, (float)(image_size_half - dot_radius), (float)(image_size_half - dot_radius), (float)(dot_radius + dot_radius), (float)(dot_radius + dot_radius));

            if (!draw_polygon)
                dot_radius += pen_width_half;
            else {
                dot_radius += pen_width_quarter;
                for (ushort i = 1; i <= petals_count; ++i)
                    gr.DrawLine(pen, (float)(image_size_half - dot_radius * Math.Sin(angle_radians_dot + angle_radians * i)), (float)(image_size_half + dot_radius * Math.Cos(angle_radians_dot + angle_radians * i)), (float)(image_size_half - dot_radius * Math.Sin(angle_radians * i - angle_radians_dot)), (float)(image_size_half + dot_radius * Math.Cos(angle_radians * i - angle_radians_dot)));
                dot_radius += pen_width_quarter;
            }

            for (ushort i = 1; i <= petals_count; ++i) {
                gfx.Clear(Color.Transparent);
                gfx.TranslateTransform(exponentRotator_size_half, exponentRotator_size_half);
                gfx.RotateTransform(angle_degrees);
                gfx.TranslateTransform(-exponentRotator_size_half, -exponentRotator_size_half);
                gfx.DrawImageUnscaled(exponent, exponentRotator_position);

                gr.DrawImageUnscaled(exponentRotator, (int)Math.Round(image_size_half_expRotator - stick * Math.Sin(angle_radians * i)), (int)Math.Round(image_size_half_expRotator + stick * Math.Cos(angle_radians * i)));

                if (draw_connhideDots) gr.FillEllipse(brush, (float)(image_size_half_dot - dot_radius * Math.Sin(angle_radians_dot + angle_radians * i)), (float)(image_size_half_dot + dot_radius * Math.Cos(angle_radians_dot + angle_radians * i)), pen_width, pen_width); //I understand that this is not optimal, but the convenience of the programmer is above all...
            }

            // Image rotating area.
            if (rotation != 0) {
                Bitmap temple = new Bitmap(image_size, image_size, PixelFormat.Format32bppArgb);
                gfx = Graphics.FromImage(temple);
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.CompositingQuality = CompositingQuality.HighQuality;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;

                if (background_alpha) gfx.Clear(Color.Transparent);
                else {
                    if (color_inverse) gfx.Clear(Color.Black);
                    else gfx.Clear(Color.White);
                }

                gfx.TranslateTransform((ushort)image_size_half, (ushort)image_size_half);
                if (!rotation_absolute) //Did you know that the else branch is often slower?
                    gfx.RotateTransform(360.0F * rotation / petals_count);
                else
                    gfx.RotateTransform(rotation);
                gfx.TranslateTransform(-(ushort)image_size_half, -(ushort)image_size_half);

                gfx.DrawImageUnscaled(image, 0, 0);

                image = temple;
                gr = Graphics.FromImage(image);
            }

            return true;
        }

        private void Cmd_Execute() {
            if (cmd.Text == "") {
                cmd_log.Items.AddRange(new string[] { "> ", "error: empty cmd" });
                cmd.Text = "";
                return;
            }
            if (cmd.Text.Contains('>')) {
                cmd_log.Items.AddRange(new string[] { "> ", "error: invalid command" });
                return;
            }

            Cmd_Execute_filtered();

            int prew_index = cmd_log.SelectedIndex;
            cmd_log.SelectedIndex = cmd_log.Items.Count - 1;
            cmd_log.SelectedIndex = prew_index;
        }
        private void Cmd_Execute_filtered() { //Guess why this split was needed? Because if I use goto, a meteorite may fall!
            string temp_string = Regex.Replace(cmd.Text.ToLower(), "[ ]{2,}", " ");
            string[] cmds = temp_string.Split(' ');

            cmd_log.Items.Add("> " + temp_string);
            cmd_log.SelectedIndex = cmd_log.Items.Count - 1;
            cmd.Text = "";

            switch (cmds[0]) {
                case "gen": {
                    if (need_pinit) {
                        cmd_log.Items.AddRange(new string[] { "error: params not initialized", "tip: use \"pinit\"" });
                        return;
                    }

                    if (!Gen()) {
                        cmd_log.Items.AddRange(new string[] { "error: flower ratio is too big", "tip: try to decrease exponent power and/or up limit" });
                        return;
                    }

                    view.Image = image;

                    cmd_log.Items.Add("info: done");
                }
                return;
                case "help":
                    cmd_log.Items.AddRange(new string[] {
            "ag {true|false} - prop - regenerate flower on prop change",
            "ai {true|false} - prop - initiate pic on prop change",
            "alpha {true|false} - prop - backgroung color mode",
            "black - func - dark theme",
            "cd {true|false} - prop - draw center dot",
            "chd {true|false} - prop - draw dots on connections",
            "cln - func - clean the log",
            "df {float, not 0} - prop - deform exponent",
            "draw - func - draw default flower",
            "eul {float 0<} - prop - exponent generator y limit",
            "gen - func - generate fllower with current props",
            "gsq {lb|eul|df|rt} {float} {float} - func - generate sequence. Format: gsq cmd max/min step",
            "help - func - show help info",
            "ir {true|false} - prop - draw inner radius",
            "iv {true|false} - prop - color inverse mode",
            "lb {e|pi|float 1<} - prop - logoriphm basis",
            "man {cmd} - func - typical usage info",
            "pc {int 3+} - prop - count of petals",
            "pinit - func - init pic",
            "pg {true|false} - prop - draw polygon",
            "pwp {float 0-100} - prop - pen width percent",
            "qa {medium|high|int 100+} - prop - set pic quality",
            "rot {float}[p] - func - rotate existing image by given degrees",
            "rt {float}[p] - prop - rotate by given degrees",
            "save - func - save image into file",
            "sh - func - show current flower props",
            "ss - func - show current props",
            "sv {path} - func - save image into specefied file",
            "white - func - light theme"});
                    return;
                case "man":
                    if (cmds.All((str) => { return str == "man"; })) {
                        cmd_log.Items.AddRange(new string[] { "typical usage:",
                           String.Join(" ", cmds) + " man"});
                    } else if (cmds.Length == 2) {
                        switch (cmds[1]) {
                            case "ag":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "ag true"});
                                break;
                            case "ai":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "ai true"});
                                break;
                            case "alpha":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "alpha true"});
                                break;
                            case "black":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "You can't use blacks!"});
                                break;
                            case "cd":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "cd true"});
                                break;
                            case "chd":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "chd false"});
                                break;
                            case "cln":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "cln"});
                                break;
                            case "df":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "df -1"});
                                break;
                            case "draw":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "draw"});
                                break;
                            case "daw":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "For recording, storing, editing, and playing digital audio."});
                                break;
                            case "eul":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "eul 1"});
                                break;
                            case "gen":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "gen"});
                                break;
                            case "gsq":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "gsq df 10 0.5"});
                                break;
                            case "help":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "help"});
                                break;
                            case "ir":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "ir true"});
                                break;
                            case "iv":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "iv true"});
                                break;
                            case "lb":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "lb pi"});
                                break;
                            case "pc":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "pc 7"});
                                break;
                            case "pinit":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "pinit"});
                                break;
                            case "pg":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "pg true"});
                                break;
                            case "pwp":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "pwp 5"});
                                break;
                            case "qa":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "qa high"});
                                break;
                            case "rot":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "rot -22.5"});
                                break;
                            case "rt":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "rt 0.5p"});
                                break;
                            case "save":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "save"});
                                break;
                            case "sh":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "sh"});
                                break;
                            case "ss":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "ss"});
                                break;
                            case "sv":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "sv ..\\image"});
                                break;
                            case "white":
                                cmd_log.Items.AddRange(new string[] { "typical usage:",
                           "For factories, mines and heavy industry."});
                                break;
                        }
                    } else
                        cmd_log.Items.Add("error: man overflow");
                    return;
                case "save":
                    if (cmds.Length != 1) break;
                    if (need_pinit) {
                        cmd_log.Items.AddRange(new string[] { "error: params not initialized", "tip: use \"pinit\"" });
                        return;
                    }
                    image.Save("eFlower.png", ImageFormat.Png);
                    cmd_log.Items.Add("info: done");
                    return;
                case "sv":
                    if (cmds.Length != 2) break;
                    if (need_pinit) {
                        cmd_log.Items.AddRange(new string[] { "error: params not initialized", "tip: use \"pinit\"" });
                        return;
                    }
                    image.Save(cmds[1] + ".png", ImageFormat.Png);
                    cmd_log.Items.Add("info: done");
                    return;
                case "gsq":
                    if (cmds.Length != 4) break; {
                        if (float.TryParse(cmds[3], out float step) && float.TryParse(cmds[2], out float end_val)) {
                            switch (cmds[1]) {
                                case "lb":
                                    if (end_val > log_basis) {
                                        cmd.Text = "pinit"; Cmd_Execute();

                                        for (; log_basis < end_val; log_basis += step) {
                                            if (!Gen()) {
                                                cmd_log.Items.AddRange(new string[] { "error: flower ratio is too big", "tip: try to decrease exponent power and/or up limit" });
                                                break;
                                            }
                                            image.Save("eFlower_lb" + Math.Round(log_basis, 4) + ".png", ImageFormat.Png);
                                        }

                                        log_basis -= step; view.Image = image;
                                        cmd_log.Items.Add("gsq: done");
                                        return;
                                    } else if (end_val < log_basis && end_val >= 1) {
                                        cmd.Text = "pinit"; Cmd_Execute();

                                        for (; log_basis > end_val; log_basis -= step) {
                                            if (!Gen()) {
                                                cmd_log.Items.AddRange(new string[] { "error: flower ratio is too big", "tip: try to decrease exponent power and/or up limit" });
                                                break;
                                            }
                                            image.Save("eFlower_lb" + Math.Round(log_basis, 4) + ".png", ImageFormat.Png);
                                        }

                                        log_basis -= step; view.Image = image;
                                        cmd_log.Items.Add("gsq: done");
                                        return;
                                    }
                                    break;
                                case "eul":
                                    if (end_val > exp_y_limit) {
                                        cmd.Text = "pinit"; Cmd_Execute();

                                        for (; exp_y_limit < end_val; exp_y_limit += step) {
                                            if (!Gen()) {
                                                cmd_log.Items.AddRange(new string[] { "error: flower ratio is too big", "tip: try to decrease exponent power and/or up limit" });
                                                break;
                                            }
                                            image.Save("eFlower_eul" + Math.Round(exp_y_limit, 4) + ".png", ImageFormat.Png);
                                        }

                                        exp_y_limit -= step; view.Image = image;
                                        cmd_log.Items.Add("gsq: done");
                                        return;
                                    } else if (end_val < exp_y_limit && end_val >= 0) {
                                        cmd.Text = "pinit"; Cmd_Execute();

                                        for (; exp_y_limit > end_val; exp_y_limit -= step) {
                                            if (!Gen()) {
                                                cmd_log.Items.AddRange(new string[] { "error: flower ratio is too big", "tip: try to decrease exponent power and/or up limit" });
                                                break;
                                            }
                                            image.Save("eFlower_eul" + Math.Round(exp_y_limit, 4) + ".png", ImageFormat.Png);
                                        }

                                        exp_y_limit -= step; view.Image = image;
                                        cmd_log.Items.Add("gsq: done");
                                        return;
                                    }
                                    break;
                                case "df":
                                    if (end_val > deform) {
                                        if (need_pinit) {
                                            cmd.Text = "pinit"; Cmd_Execute();
                                        }

                                        for (; deform < end_val; deform += step) {
                                            if (deform != 0) {
                                                if (!Gen()) {
                                                    cmd_log.Items.AddRange(new string[] { "error: flower ratio is too big", "tip: try to decrease exponent power and/or up limit" });
                                                    break;
                                                }
                                                image.Save("eFlower_df" + Math.Round(deform, 4) + ".png", ImageFormat.Png);
                                            }
                                        }

                                        deform -= step; view.Image = image;
                                        cmd_log.Items.Add("gsq: done");
                                        return;
                                    } else if (end_val < deform) {
                                        cmd.Text = "pinit"; Cmd_Execute();

                                        for (; deform > end_val; deform -= step) {
                                            if (deform != 0) {
                                                if (!Gen()) {
                                                    cmd_log.Items.AddRange(new string[] { "error: flower ratio is too big", "tip: try to decrease exponent power and/or up limit" });
                                                    break;
                                                }
                                                image.Save("eFlower_df" + Math.Round(deform, 4) + ".png", ImageFormat.Png);
                                            }
                                        }

                                        deform -= step; view.Image = image;
                                        cmd_log.Items.Add("gsq: done");
                                        return;
                                    }
                                    break;
                                case "rt":
                                    if (end_val > rotation) {
                                        cmd.Text = "pinit"; Cmd_Execute();

                                        for (; rotation < end_val; rotation += step) {
                                            if (!Gen()) {
                                                cmd_log.Items.AddRange(new string[] { "error: flower ratio is too big", "tip: try to decrease exponent power and/or up limit" });
                                                break;
                                            }
                                            image.Save("eFlower_lb" + Math.Round(rotation, 4) + ".png", ImageFormat.Png);
                                        }

                                        rotation -= step; view.Image = image;
                                        cmd_log.Items.Add("gsq: done");
                                        return;
                                    } else if (end_val < rotation) {
                                        cmd.Text = "pinit"; Cmd_Execute();

                                        for (; rotation > end_val; rotation -= step) {
                                            if (!Gen()) {
                                                cmd_log.Items.AddRange(new string[] { "error: flower ratio is too big", "tip: try to decrease exponent power and/or up limit" });
                                                break;
                                            }
                                            image.Save("eFlower_lb" + Math.Round(rotation, 4) + ".png", ImageFormat.Png);
                                        }

                                        rotation -= step; view.Image = image;
                                        cmd_log.Items.Add("gsq: done");
                                        return;
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case "draw":
                    cmd.Text = "ai 1";
                    Cmd_Execute();
                    cmd.Text = "ag 1";
                    Cmd_Execute();
                    cmd.Text = "pinit";
                    Cmd_Execute();
                    return;
                case "daw": {
                    cmd_log.Items.Add("info: I haven't a daw, but I have something for you");

                    byte num = (byte)(DateTime.Now.Ticks % 4);

                    Stream wave = (num == 0 ? Properties.Resources.K_391___Electro_House :
                        (num == 1 ? Properties.Resources.Beautiful_Day__Nordic_Remix_ :
                        (num == 2 ? Properties.Resources.Russia_Privjet__Chromeboy_2nd_Remix_ :
                        Properties.Resources.E_Type___Until_The_End)));

                    System.Media.SoundPlayer sp = new System.Media.SoundPlayer(wave);
                    sp.Play();
                }
                return;
                case "saw": {
                    cmd_log.Items.Add("easteregg: You like electronic music, right? ;)");

                    System.Media.SoundPlayer sp = new System.Media.SoundPlayer(Properties.Resources.saw);
                    sp.Play();
                }
                return;
                case "sh":
                    if (cmds.Length != 1) break;
                    cmd_log.Items.AddRange(new string[] {
                    "log basis: " + (log_basis == Math.E ? "e" : (log_basis == Math.PI ? "pi" : log_basis.ToString())),
                    "exponent height limit: " + exp_y_limit,
                    "deform: " + deform,
                    "rotation: " + (rotation_absolute ? rotation.ToString() : rotation + "p"),
                    "petals count: " + petals_count,
                    "mask connections: " + draw_connhideDots.ToString(),
                    "draw center dot: " + draw_centerDot.ToString(),
                    "draw inner radius: " + draw_innerCircle.ToString(),
                    "draw polygon: " + draw_polygon.ToString()});
                    return;
                case "ss":
                    if (cmds.Length != 1) break;

                    if (image_size == 1024)
                        temp_string = "medium";
                    else if (image_size == 16384)
                        temp_string = "high";
                    else
                        temp_string = "custom";

                    cmd_log.Items.AddRange(new string[] {
                    "pic quality: " + temp_string,
                    "pen width: " + pen_width_percent + "%",
                    "alpha: " + background_alpha.ToString(),
                    "inverse: " + color_inverse.ToString(),

                    "autoregen: " + regen.ToString(),
                    "autoreinit: " + reinit.ToString()});
                    return;
                case "cln":
                    cmd_log.Items.Clear();
                    return;
                case "qa":
                    if (cmds.Length == 2) {
                        switch (cmds[1]) { // We should avoid goto! This is necessary! If I write goto, everyone will immediately stop understanding the code of this program. It's a good thing it's C# and not like Assembly language.
                            case "medium":
                            case "mid":
                                image_size = 1024;
                                cmd_log.Items.Add("info: reinit need");

                                need_pinit = true;
                                if (reinit) {
                                    cmd.Text = "pinit";
                                    Cmd_Execute();
                                }
                                return;
                            case "high":
                            case "hg":
                                image_size = 16384;
                                cmd_log.Items.Add("info: reinit need");

                                need_pinit = true;
                                if (reinit) {
                                    cmd.Text = "pinit";
                                    Cmd_Execute();
                                }
                                return;
                            default:
                                if (ushort.TryParse(cmds[1], out ushort temp) && temp >= 100) {
                                    image_size = temp;
                                    cmd_log.Items.Add("info: reinit need");

                                    need_pinit = true;
                                    if (reinit) {
                                        cmd.Text = "pinit";
                                        Cmd_Execute();
                                    }
                                    return;
                                }
                                break;
                        }
                    }
                    break;
                case "lb":
                    if (cmds.Length == 2) {
                        if (double.TryParse(cmds[1], out double temp)) {
                            if (temp > 1) {
                                log_basis = temp;
                                cmd_log.Items.Add("info: regen need");

                                if (regen) {
                                    cmd.Text = "gen";
                                    Cmd_Execute();
                                }
                                return;
                            }
                        } else if (cmds[1] == "e") {
                            log_basis = Math.E;
                            cmd_log.Items.Add("info: regen need");

                            if (regen) {
                                cmd.Text = "gen";
                                Cmd_Execute();
                            }
                            return;
                        } else if (cmds[1] == "pi") {
                            log_basis = Math.PI;
                            cmd_log.Items.Add("info: regen need");

                            if (regen) {
                                cmd.Text = "gen";
                                Cmd_Execute();
                            }
                            return;
                        }
                    }
                    break;
                case "ag":
                    if (cmds.Length == 2 && RecogniseBool(cmds[1], ref regen)) {
                        cmd_log.Items.Add("info: done");
                        return;
                    }
                    break;
                case "ai":
                    if (cmds.Length == 2 && RecogniseBool(cmds[1], ref reinit)) {
                        cmd_log.Items.Add("info: done");
                        return;
                    }
                    break;
                case "alpha":
                    if (cmds.Length == 2 && RecogniseBool(cmds[1], ref background_alpha)) {
                        cmd_log.Items.Add("info: reinit need");

                        need_pinit = true;
                        if (reinit) {
                            cmd.Text = "pinit";
                            Cmd_Execute();
                        }
                        return;
                    }
                    break;
                case "iv":
                    if (cmds.Length == 2 && RecogniseBool(cmds[1], ref color_inverse)) {
                        cmd_log.Items.Add("info: reinit need");

                        need_pinit = true;
                        if (reinit) {
                            cmd.Text = "pinit";
                            Cmd_Execute();
                        }
                        return;
                    }
                    break;
                case "chd":
                    if (cmds.Length == 2 && RecogniseBool(cmds[1], ref draw_connhideDots)) {
                        cmd_log.Items.Add("info: regen need");

                        if (regen) {
                            cmd.Text = "gen";
                            Cmd_Execute();
                        }
                        return;
                    }
                    break;
                case "cd":
                    if (cmds.Length == 2 && RecogniseBool(cmds[1], ref draw_centerDot)) {
                        cmd_log.Items.Add("info: regen need");

                        if (regen) {
                            cmd.Text = "gen";
                            Cmd_Execute();
                        }
                        return;
                    }
                    break;
                case "ir":
                    if (cmds.Length == 2 && RecogniseBool(cmds[1], ref draw_innerCircle)) {
                        cmd_log.Items.Add("info: regen need");

                        if (regen) {
                            cmd.Text = "gen";
                            Cmd_Execute();
                        }
                        return;
                    }
                    break;
                case "pg":
                    if (cmds.Length == 2 && RecogniseBool(cmds[1], ref draw_polygon)) {
                        cmd_log.Items.Add("info: regen need");

                        if (regen) {
                            cmd.Text = "gen";
                            Cmd_Execute();
                        }
                        return;
                    }
                    break;
                case "pwp":
                    if (cmds.Length == 2) {
                        if (float.TryParse(cmds[1], out float temp)) {
                            if (temp > 0 && temp < 100) {
                                pen_width_percent = temp;
                                cmd_log.Items.Add("info: reinit need");

                                need_pinit = true;
                                if (reinit) {
                                    cmd.Text = "pinit";
                                    Cmd_Execute();
                                }
                                return;
                            }

                        }
                    }
                    break;
                case "df":
                    if (cmds.Length == 2) {
                        if (float.TryParse(cmds[1], out float temp)) {
                            if (temp != 0) {
                                deform = temp;
                                cmd_log.Items.Add("info: regen need");

                                if (regen) {
                                    cmd.Text = "gen";
                                    Cmd_Execute();
                                }
                                return;
                            }
                        }
                    }
                    break;
                case "eul":
                    if (cmds.Length == 2) {
                        if (float.TryParse(cmds[1], out float temp)) {
                            if (temp > 0) {
                                exp_y_limit = temp;
                                cmd_log.Items.Add("info: regen need");

                                if (regen) {
                                    cmd.Text = "gen";
                                    Cmd_Execute();
                                }
                                return;
                            }

                        }
                    }
                    break;
                case "pc":
                    if (cmds.Length == 2) {
                        if (byte.TryParse(cmds[1], out byte temp)) {
                            if (temp > 2) {
                                petals_count = temp;
                                cmd_log.Items.Add("info: regen need");

                                if (regen) {
                                    cmd.Text = "gen";
                                    Cmd_Execute();
                                }
                                return;
                            }
                        }
                    }
                    break;
                case "rt":
                    if (cmds.Length == 2) {
                        if (float.TryParse(cmds[1], out float temp)) {
                            rotation = temp;
                            rotation_absolute = true;

                            cmd_log.Items.Add("info: regen need");
                            if (regen) {
                                cmd.Text = "gen";
                                Cmd_Execute();
                            }
                            return;
                        } else if (cmds[1].EndsWith("p") && float.TryParse(cmds[1].Remove(cmds[1].Length - 1), out temp)) {
                            rotation = temp;
                            rotation_absolute = false;

                            cmd_log.Items.Add("info: regen need");
                            if (regen) {
                                cmd.Text = "gen";
                                Cmd_Execute();
                            }
                            return;
                        }
                    }
                    break;
                case "rot":
                    if (need_pinit) {
                        cmd_log.Items.AddRange(new string[] { "error: params not initialized", "tip: use \"pinit\"" });
                        return;
                    }
                    if (cmds.Length == 2) {
                        if (float.TryParse(cmds[1], out float temp)) {
                            Bitmap temple = new Bitmap(image_size, image_size, PixelFormat.Format32bppArgb);
                            Graphics gfx = Graphics.FromImage(temple);
                            float image_size_half = image_size / 2.0F;
                            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            gfx.CompositingQuality = CompositingQuality.HighQuality;
                            gfx.SmoothingMode = SmoothingMode.AntiAlias;

                            if (background_alpha) gfx.Clear(Color.Transparent);
                            else {
                                if (color_inverse) gfx.Clear(Color.Black);
                                else gfx.Clear(Color.White);
                            }

                            gfx.TranslateTransform(image_size_half, image_size_half);
                            gfx.RotateTransform(temp);
                            gfx.TranslateTransform(-image_size_half, -image_size_half);

                            gfx.DrawImageUnscaled(image, 0, 0);

                            view.Image = image = temple;
                            gr = Graphics.FromImage(image);

                            cmd_log.Items.Add("info: done");
                            return;
                        } else if (cmds[1].EndsWith("p") && float.TryParse(cmds[1].Remove(cmds[1].Length - 1), out temp)) {
                            Bitmap temple = new Bitmap(image_size, image_size, PixelFormat.Format32bppArgb);
                            Graphics gfx = Graphics.FromImage(temple);
                            float image_size_half = image_size / 2.0F;
                            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            gfx.CompositingQuality = CompositingQuality.HighQuality;
                            gfx.SmoothingMode = SmoothingMode.AntiAlias;

                            if (background_alpha) gfx.Clear(Color.Transparent);
                            else {
                                if (color_inverse) gfx.Clear(Color.Black);
                                else gfx.Clear(Color.White);
                            }

                            gfx.TranslateTransform(image_size_half, image_size_half);
                            gfx.RotateTransform(360.0F * temp / petals_count);
                            gfx.TranslateTransform(-image_size_half, -image_size_half);

                            gfx.DrawImageUnscaled(image, 0, 0);

                            view.Image = image = temple;
                            gr = Graphics.FromImage(image);

                            cmd_log.Items.Add("info: done");
                            return;
                        }
                    }
                    break;
                case "pinit":
                    if (cmds.Length != 1) break;

                    image = new Bitmap(image_size, image_size, PixelFormat.Format32bppArgb);
                    gr = Graphics.FromImage(image);
                    gr.SmoothingMode = SmoothingMode.HighQuality;
                    gr.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    gr.CompositingQuality = CompositingQuality.HighQuality;

                    if (color_inverse) {
                        if (!background_alpha)
                            gr.Clear(Color.Black);

                        pen = new Pen(brush = Brushes.White, image_size * pen_width_percent / 400);
                    } else {
                        if (!background_alpha)
                            gr.Clear(Color.White);

                        pen = new Pen(brush = Brushes.Black, image_size * pen_width_percent / 400);
                    }

                    view.Image = image;
                    cmd_log.Items.Add("info: done");

                    need_pinit = false;
                    if (regen) {
                        cmd.Text = "gen";
                        Cmd_Execute();
                    }
                    return;
                case "black":
                    if (cmds.Length == 1) {
                        cmd_log.Items.AddRange(new string[] { "shame: racist word", "tip: use with \"master\"" });
                    } else if (cmds.Length == 2 && cmds[1] == "master") {
                        cmd.BackColor = button1.BackColor = cmd_log.BackColor = splitContainer1.Panel2.BackColor = Color.Black;
                        cmd.ForeColor = cmd_log.ForeColor = button1.ForeColor = label1.ForeColor = label2.ForeColor = Color.White;
                        if (cmd.ForeColor == Color.White)
                            cmd_log.Items.Add("good");
                        else
                            cmd_log.Items.Add("theme changed");
                    } else break;
                    return;
                case "white":
                    if (cmds.Length == 1) {
                        cmd_log.Items.AddRange(new string[] { "shame: racist word", "tip: use with \"slave\"" });
                    } else if (cmds.Length == 2 && cmds[1] == "slave") {
                        if (cmd.ForeColor == Color.White) {
                            cmd_log.Items.AddRange(new string[] { "shame: racist act", "Why you want theme changed? Do you despise blacks?" });
                        } else
                            cmd_log.Items.Add("good");
                    } else break;
                    return;
                default:
                    cmd_log.Items.Add("error: invalid command");
                    return;
            }
            cmd_log.Items.Add("error: invalid syntax");
        }
    }
}
