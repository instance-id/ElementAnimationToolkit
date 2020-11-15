# The basics

### Usage:

Note: The code is pretty heavily documented and the summaries for extension methods have examples in them.

Important files:


    Assets/instance.id/Element Animation Toolkit/UIElementsAnimationEditor.cs
This file is the primary example and demonstratitive reference for most major features and is the main editor window of the package where you can view examples of several different types of animations and their usage.

There are three buttons per row, Editor, Anim, and USS. The editor button will take you directly to the editor code specific to that rows animation where you will see the C# implementation of UIElements as well as most callback registrations. The Anim button takes you to another section of the file in which you can see the declaration, setup, and execution of any animation specific functions, and lastly is the USS button, which takes you to the USS stylesheet and the location of the particular animation if you need. The

The animations without a row of buttons have a right-click context menu implemented which has similar options to jump directly into the code at the proper location for the animation. (The methods I have created for jumping straight to the proper lines of code are of course included and you are welcome to take advantage of them for your own needs.)