using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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

        float exp_y_limit = 4;
        byte petals_count = 8;
        double log_basis = Math.E;
        bool draw_connhideDots = true;

        Graphics gr;
        Pen pen; Brush brush;
        Bitmap image;
        bool regen = false, reinit = false, need_pinit = true;
        bool background_alpha = false;
        ushort image_size = 1024; //Square
        float pen_width_percent = 4;

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
                printToE = exp_y_limit / ebheight,
                eToPrint = ebheight / exp_y_limit;

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

            egr.FillEllipse(brush, (float)ebwidtHalf, dot_y - pen_width_half, pen_width, pen_width);
            for (ushort i = 1; i < ebwidtHalf; ++i) {
                ebwidtHalf_offset = (float)(ebwidtHalf + i + pen_width_half);
                egr.DrawLine(pen, ebwidtHalf_offset - 1, dot_y, ebwidtHalf_offset,
                    dot_y = (float)(ebheight - (Math.Pow(log_basis, i * printToE) - 1) * eToPrint));
                egr.FillEllipse(brush, ebwidtHalf_offset - pen_width_half, dot_y - pen_width_half, pen_width, pen_width);
            }

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
            else gr.Clear(Color.White);

            dot_radius += pen_width_half;

            for (ushort i = 1; i <= petals_count; ++i) {
                gfx.Clear(Color.Transparent);
                gfx.TranslateTransform(exponentRotator_size_half, exponentRotator_size_half);
                gfx.RotateTransform(angle_degrees);
                gfx.TranslateTransform(-exponentRotator_size_half, -exponentRotator_size_half);
                gfx.DrawImageUnscaled(exponent, exponentRotator_position);

                gr.DrawImageUnscaled(exponentRotator, (int)Math.Round(image_size_half_expRotator - stick * Math.Sin(angle_radians * i)), (int)Math.Round(image_size_half_expRotator + stick * Math.Cos(angle_radians * i)));

                if (draw_connhideDots) gr.FillEllipse(brush, (float)(image_size_half_dot - dot_radius * Math.Sin(angle_radians_dot + angle_radians * i)), (float)(image_size_half_dot + dot_radius * Math.Cos(angle_radians_dot + angle_radians * i)), pen_width, pen_width); //I understand that this is not optimal, but the convenience of the programmer is above all...
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
        private void Cmd_Execute_filtered() {
            string temp_string = Regex.Replace(cmd.Text.ToLower(), "[ ]{2,}", " ");
            string[] cmds = temp_string.Split(' ');

            cmd_log.Items.Add("> " + temp_string);
            cmd_log.SelectedIndex = cmd_log.Items.Count - 1;
            cmd.Text = "";

            switch (cmds[0]) {
                case "gen": {
                    if (need_pinit) {
                        cmd_log.Items.AddRange(new string[] { "error: params not initialized", "tip: use \"init\"" });
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
            "alpha {true|false} - prop - backgroung color mode",
            "autoinit {true|false} - prop - initiate pic on prop change",
            "autoregen {true|false} - prop - regenerate flower on prop change",
            "chide {true|false} - prop - draw dots on connections",
            "clean - func - clean the log",
            "eyl {float 0<} - prop - exponent generator y limit",
            "example - func - draw default flower",
            "gen - func - generate flower with current props",
            "help - func - show help info",
            "init - func - init pic",
            "lb {e|pi|float 1<} - prop - logoriphm basis",
            "pc {int 3+} - prop - count of petals",
            "pwp {float 0-100} - prop - pen width percent",
            "quality {medium|high|int 100+} - prop - set pic quality",
            "save - func - save image into file",
			"savep {path} - func - save image into specefied file",
            "sfp - func - show flower props",
            "spp - func - show program props"});
                    return;
                case "save":
                    if (cmds.Length != 1) break;
                    if (need_pinit) {
                        cmd_log.Items.AddRange(new string[] { "error: params not initialized", "tip: use \"init\"" });
                        return;
                    }
                    image.Save("eFlower.png", ImageFormat.Png);
                    cmd_log.Items.Add("info: done");
                    return;
                case "savep":
                    if (cmds.Length != 2) break;
                    if (need_pinit) {
                        cmd_log.Items.AddRange(new string[] { "error: params not initialized", "tip: use \"init\"" });
                        return;
                    }
                    image.Save(cmds[1] + ".png", ImageFormat.Png);
                    cmd_log.Items.Add("info: done");
                    return;
                case "example":
                    cmd.Text = "autoinit 1";
                    Cmd_Execute();
                    cmd.Text = "autoregen 1";
                    Cmd_Execute();
                    cmd.Text = "init";
                    Cmd_Execute();
                    return;
                case "sfp":
                    if (cmds.Length != 1) break;
                    cmd_log.Items.AddRange(new string[] {
                    "log basis: " + (log_basis == Math.E ? "e" : (log_basis == Math.PI ? "pi" : log_basis.ToString())),
                    "exponent height limit: " + exp_y_limit,
                    "petals count: " + petals_count,
                    "mask connections: " + draw_connhideDots.ToString()});
                    return;
                case "spp":
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

                    "autoregen: " + regen.ToString(),
                    "autoreinit: " + reinit.ToString()});
                    return;
                case "clean":
                    cmd_log.Items.Clear();
                    return;
                case "quality":
                    if (cmds.Length == 2) {
                        switch (cmds[1]) {
                            case "medium":
                            case "mid":
                                image_size = 1024;
                                cmd_log.Items.Add("info: reinit need");

                                need_pinit = true;
                                if (reinit) {
                                    cmd.Text = "init";
                                    Cmd_Execute();
                                }
                                return;
                            case "high":
                            case "hg":
                                image_size = 16384;
                                cmd_log.Items.Add("info: reinit need");

                                need_pinit = true;
                                if (reinit) {
                                    cmd.Text = "init";
                                    Cmd_Execute();
                                }
                                return;
                            default:
                                if (ushort.TryParse(cmds[1], out ushort temp) && temp >= 100) {
                                    image_size = temp;
                                    cmd_log.Items.Add("info: reinit need");

                                    need_pinit = true;
                                    if (reinit) {
                                        cmd.Text = "init";
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
                case "autoregen":
                    if (cmds.Length == 2 && RecogniseBool(cmds[1], ref regen)) {
                        cmd_log.Items.Add("info: done");
                        return;
                    }
                    break;
                case "autoinit":
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
                            cmd.Text = "init";
                            Cmd_Execute();
                        }
                        return;
                    }
                    break;
                case "chide":
                    if (cmds.Length == 2 && RecogniseBool(cmds[1], ref draw_connhideDots)) {
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
                                    cmd.Text = "init";
                                    Cmd_Execute();
                                }
                                return;
                            }

                        }
                    }
                    break;
                case "eyl":
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
                case "init":
                    if (cmds.Length != 1) break;

                    image = new Bitmap(image_size, image_size, PixelFormat.Format32bppArgb);
                    gr = Graphics.FromImage(image);
                    gr.SmoothingMode = SmoothingMode.HighQuality;
                    gr.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    gr.CompositingQuality = CompositingQuality.HighQuality;

                    if (!background_alpha)
                        gr.Clear(Color.White);

                    pen = new Pen(brush = Brushes.Black, image_size * pen_width_percent / 400);

                    view.Image = image;
                    cmd_log.Items.Add("info: done");

                    need_pinit = false;
                    if (regen) {
                        cmd.Text = "gen";
                        Cmd_Execute();
                    }
                    return;
                default:
                    cmd_log.Items.Add("error: invalid command");
                    return;
            }
            cmd_log.Items.Add("error: invalid syntax");
        }
    }
}
