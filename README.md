# Heightmaps

A collection of heightmap generators, along with some bmp/ppm image format savers.

Currently, [diamond-square](https://en.wikipedia.org/wiki/Diamond-square_algorithm) has been implemented, with the PPM format and SDL renderer.

The entry point is the CLI console project. This takes command-line arguments to configure (instructions can be seen by running it with 'help', or seeing the definition in the code [here](https://github.com/ChrisPritchard/Heightmaps/blob/master/cli/Program.fs#L33).

If bitmap or ppm is *not* specified, the default is to render the generated image on a SDL surface. If so, the image can be regenerated using the R key, or the program can be quit using the escape key.

The version of SDL shipped with this is for 64bit windows. Replacing the SDL.dll file with versions for other operating systems should work fine. Other versions can be found [here](https://www.libsdl.org/download-2.0.php).