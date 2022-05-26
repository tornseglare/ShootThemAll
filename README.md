# ShootThemAll
Monogame with .NET 6.0. 

Each version can be found as a git tag, making it easy to get started if you are new to Monogame. Just click the tags, select a version and download the zip file. 

This code is rebuilt from the NeonShooter demo by SimonDarksideJ, found here: https://github.com/MonoGame/MonoGame.Samples

Please note that you must install .NET Core 3.1 Runtime (LTS) for monogame to compile.

# V1.7
Using the matrix rotation to rotate some objects around a center on the screen. See the RotateSomeSkulls() function. The matrix is sent to the spriteBatch.Begin() function and affects all Draw() calls afterwards. 

* Please see how I position the objects in the LoadContent(), this helps spreading the objects around the rotated center.
* The DrawFlower() function explains the basics of rotation and scale and draws you a flower.

# V1.6
Now we are working with Matrixes to position and rotate sprites around a center. 

Please note that this version is stripped clean down to about V1.1 in order to keep the code as clean as possible. 

See lines 88 to 117 in ShootThemAllGame.cs, where the magic happens.

# V1.5
This version has several smaller (and bigger) changes:
* GetDisplayModeBestMatch(w,h) which gives the lowest available resolution equalling or larger than the requested w,h, or the largest possible resolution if w,h cannot be satisfied.
* Window can now be positioned. 
* Disabled PublishReadyToRun and TieredCompilation to improve runtime speed.
* Using SpriteSortMode.Deferred instead of Texture since you typically want the sprites drawn on top of each other in the order you call Draw() on each.
* Setting IsFixedTimeStep to false means the screen gets redrawn as fast as possible instead of a maximum of 60fps.
* Enabled mipmapping. Created an array of objects with position and scale to show the effect of scaling down a texture with mipmapping enabled.

# V1.4
Fullscreen mode with different resolutions, windowed mode with different sizes and free resizing. 
See lines 42-46 in ShootThemAllGame.cs, they let you test fullscreen and windowed mode quickly.

# V1.3
Drawing squares with changing transparency (alpha) using the NonPremultiplied BlendState.

Good reading:
https://community.monogame.net/t/how-to-make-part-of-texture-transparent-with-setdata/14555

# V1.2
Drawing transparent image multiple times. Drawing text. 
Displays framerate, increases number of images drawn over time to see when framerate drops.

# V1.1
Loads and displays an image, using the nuget MonoGame.Content.Builder.Task.

# V1.0
Minimum amount of code for a Monogame window in .NET 6.0.
