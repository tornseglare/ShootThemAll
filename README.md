# ShootThemAll
Monogame with .NET 6.0. 

Each version can be found as a git tag, making it easy to get started if you are new to Monogame. Just click the tags, select a version and download the zip file. 

This code is rebuilt from the NeonShooter demo by SimonDarksideJ, found here: https://github.com/MonoGame/MonoGame.Samples

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
