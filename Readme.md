# Mrag2

A Monogame helper library for graphics, content, and animation.

This library has seen many years of development and iteration, including different authors like
[Bromvlieg](https://github.com/Bromvlieg) and [Hazzytje](https://github.com/Hazzytje). It has
been used in a lot of unreleased internal projects for college purposes, as well as several
Ludum Dare games.

It has been licensed under the MIT license, so you are free to do what you want with this.

## Features

Below is a summary of features in this library. It is not extensive, and contains things that
aren't great and probably should be removed.

* Rendering
  * **Mrag2.CustomSpriteBatch**: Extended SpriteBatch that allows for camera movements and zooming (using scale) and some other useful features such as basic lighting.
  * **Mrag2.LightSource**: A light source for CustomSpriteBatch.
  * **Mrag2.BMFont**: Loads and renders fonts generated with BMFont.
  * **Mrag2.SpecialColor**: Static class containing some special color values.
* Math
  * **Mrag2.Easing**: Lots of easing functions for smooth animations.
  * **Mrag2.Line**: Class defining a line segment between point A and B.
  * **Mrag2.RayTrace**: Class to perform a RayTrace. Make sure you derive objects you want to be able to hit from RayBox and add them to the static RayBoxes field in this class. (This needs improvement, could use a QuadTree for example)
  * **Mrag2.RotRect**: Rotatable rectangle.
  * **Mrag2.SpecialMath**: Static class with some useful Math functions.
    * SmoothStep and SmootherStep (see also **Mrag2.Easing**)
    * IntersectionPoint
    * LineSegmentsIntersect and LineSegmentsIntersectAt
    * NormalizedOrientationFromDegrees
* Pathfinding (AStar implementation)
  * **Mrag2.Pathfinding.PathFinding**: Class for finding a path from point A to B.
* Paths
  * **Mrag2.Path**: Class used for automated paths, which can be smoothed or a bezier curve.
* XML
  * **Mrag2.XML.XmlFile**: Custom XML parser which stores its tags in-memory for easy access.
  * **Mrag2.XML.XmlTag**: A tag in an XML file.
* Input
  * **Mrag2.Input**: Static class for checking button states. Contains methods like KeyDown, KeyPressed, KeyReleased.
* Tools
  * **Mrag2.ContentRegister**: Static class that can be used for easy cached content:
    * AnimationSheet and AnimationSheetCollection
    * BMFont
    * Generated gradient textures
    * Texture2D
    * SpriteFont
    * SoundEffect
    * Song
  * **Mrag2.AnimationSheet**: Define spritesheets as animation for rendering from an XML file.
  * **Mrag2.Checksum**: Compute hashes from files or strings.
  * **Mrag2.Config**: Simple parsing of ini file-formats.
  * **Mrag2.Hacks**: Extension methods for interacting with private members of objects.
  * **Mrag2.JSON**: Json encoding/decoding implementation, based on an implementation by NHaml.
  * **Mrag2.LibraryChecks**: Performs a check whether Monogame is installed on the system. This is useful for detecting errors before the game launches.
  * **Mrag2.PrecisionStopwatch**: High precision stopwatch, able to measure time in microseconds.
* Extensions
  * **Mrag2.Extensions**
    * string: ParseVector, ParseRectangle
    * StreamReader: ReadChar, PeekChar, Seek, ReadString, ReadUntil, Expect
* GUI (experimental)
  * **Mrag2.GUI**: Static class for an "immediate" GUI API.
* 3D (experimental)
  * **Mrag2._3D**: A very basic 3D rendering class.
  * **Mrag2._Model**: A 3D model.
  * **Mrag2._Texture**: A 3D texture.

## Getting started

For an example project, take a look at the demo project in the `Mrag2Demo` folder. To summarize it:

1. In `LoadContent`, make a `CustomSpriteBatch` if you wish, then call `Mrag.Initialize(...)`.
2. In `Update`, call `Mrag.Update()`.
3. In `Draw`, call `Mrag.Render()`.

## Contributing

Pull requests are accepted and this library should be considered under constant active development.

## License

Copyright (c) 2017 Nimble Tools

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
