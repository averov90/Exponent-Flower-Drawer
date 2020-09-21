# Exponent Flower Drawer
[![License](https://img.shields.io/badge/LICENSE-LGPL%20v2.1-green?style=flat-square)](/LICENSE)  [![Version](https://img.shields.io/badge/VERSION-RELEASE%20--%201.0-green?style=flat-square)](https://github.com/averov90/Exponent-Flower-Drawer/releases/tag/1.0)
### :small_orange_diamond: [Русская версия](/README.md)

This program is designed to generate flower-like shapes using the exponent shape.
An example of how the program works is shown in the figure below (this is exactly what the program created):
![Example of the work result](https://github.com/averov90/Exponent-Flower-Drawer/raw/master/logo.png)


The program is a tool for generating exponential colors with specified parameters, such as the base of the exponent, the height of the exponent, and the deformation of the exponent. 
It is worth noting that a positive-sign strain virtually changes the base and height of the exponent simultaneously, so if you need a specific shape, you should not use the strain.
When you change the base of the exponent and its height, the inner radius of the flower (the distance from the center to the petals) also changes. If you need to change the shape of the petals without changing the radius, use a warp.

The program has quite a few small but useful settings. For example, there is a parameter that allows you to automatically draw the inner circle (by default, it will not be drawn).

## Description
Like any other program, this program should contain some description of how it works to make it easier to get started.
Many developers do this in the official style, but in this case, I would like to use the "humorous" style.

### Description in a joking form
This is a typical program with a command interface. But this interface is not in the console window, but in the graphical one. There is no command line-there is only a field for entering commands and a field for displaying output.
Like any program with a command interface, this program has a set of commands. Like any program with a command interface, this set is unique.
It is not always immediately clear why the team looks like this, sometimes there are problems with understanding the logic of the person who made the abbreviations.
For some reason, compilers of team lists like to invent words, as well as give a completely different meaning to existing abbreviations.
When working with this program, you will definitely have misunderstandings. Well, trial and error will help you.

Each program writes help in its own way. Well, here `help` is also peculiar, as well as `man`.
The creators of team interfaces really like to do it their own way, and not the way other people are used to seeing it. Sometimes you think that everything is clear, but it turns out exactly the opposite.
Yes, and the rules for processing commands are very different, as well as the relationship of commands that still need to be understood. 
However, the logic in all this kotovasia is still there and when you understand it, you will suddenly become much easier. With this program. 
And others have a different logic - it has yet to be comprehended.

### Now seriously
From the description above, you might think that the program presented in the repository is some kind of joke-toy, but no - this is a real program that can actually create what is shown in the picture above and not only that.
You can adjust the thickness of lines, the quality (resolution) of the image, the base of the exponent, the virtual height of the exponent, its deformation, the number of petals, and the rotation of the resulting drawing.
There are also auxiliary functions, such as masking the connections of the petals.
Of course, you can save the resulting image in png (the program works with alpha), 
you can also generate a sequence of images with a single variable parameter (set the final value and step). This can be useful for pre-generating a set of options and filtering out unsuitable ones, as well as for creating animations.
*If you are going to use this program in your work and you are not satisfied with the reduction of commands in the program, create an* ***Issue*** *- I will review, make changes, and release an additional release.*

### At first
1. To get started, download the program and run it - no installation is required
2. See the list of commands displayed in the output field
3. Enter commands in the command field, then press either the button on the right or enter.

It will be easier to get started if you enter `draw` as the first command. This command is expanded into a combination that immediately displays the shape (the output window will also display the commands that this command executed in the order of execution). Then you can simply change the parameters you are interested in - **prop** - and look at the result.

Also, you can place the executed command (prefixed with `>`) from the output field to the input field by double-clicking, or by pressing enter when the output field is in focus.

By the way, the decimal separator in this program is a dot (`3.14`).

# P. S.

  It is also important to say that this project did not stand aside in our difficult times with our, without exaggeration, the most important problem of our time on a global scale - the problem of racism.
  More precisely, the problem of its existence. The program highlights the absolute and unprecedented incorrectness of the racial conflict fueled by the vicious social stereotype of pale people, and calls for the most global measures.
  For example, it is completely unfair that the rules of chess say that it is white who goes first. It would be much more correct if the blacks went first, because they have been second for so many years.
  It is also worth looking in the direction of radio engineering, where 0 (earth) is indicated in black, which, of course, is unfair. 
  It would be much more correct to start the black wires with the same potential as the white ones. Also, as a solution, it is proposed to cancel the black and white wires with the transition to gray.
  You can also remember the paper. Why is the paper white? Why is the ratio of white and black in books not equal? Why is "black" the color of darkness, and the brightest light itself "white"? Why is light a presence and darkness an absence?!
  I think physicists should solve this problem: we just need lamps of black light and white shadow (white shadow is not necessary).
  As you can see, the problem exists at the most fundamental level and is not at all exaggerated and, moreover, is not sucked out of the finger for any purpose.

  It is too impossible for people to accept differences and not try to equalize everything as if it were an equation (without coefficients-only racists have coefficients).
  The fact is that in human nature (the corresponding part of the spinal cord was discovered), it is necessary to assume that if something is different, then it is definitely either better or worse.
  The concept of "just another", or, more scientifically, "parallel" does not exist - geometry lied to you.

  Remember: if something is "different", it is definitely either worse or better. If it is neither better nor worse, then it is not "different". Even when we compare cheese to a chair.

###### And yes, I committed it to master.
