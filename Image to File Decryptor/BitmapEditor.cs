using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace Image_to_File_Decryptor
{
    /// <summary>
    /// Class which represents a new bitmap with some added functionallity and helper methods on top for specific help with drawing 
    /// the natural objects.
    /// </summary>
    class BitmapEditor
    {
        Bitmap newMap;

        // Define the colors which will be used.
        private readonly Color largeTree = Color.FromArgb(0, 153, 51);
        private readonly Color mediumTree = Color.FromArgb(153, 204, 102);
        private readonly Color smallTree = Color.FromArgb(204, 255, 153);
        private readonly Color largeRock = Color.FromArgb(153, 153, 153);
        private readonly Color mediumRock = Color.FromArgb(204, 204, 204);
        private readonly Color smallRock = Color.FromArgb(255, 255, 255);

        /// <summary>
        /// Constructs a new bitmap with the given parameters.
        /// </summary>
        /// <param name="width"> The width of the new image </param>
        /// <param name="height"> The height of the new image </param>
        public BitmapEditor(int width, int height)
        {
            newMap = new Bitmap(width, height);
        }

        /// <summary>
        /// Will draw the specific object onto the image.
        /// </summary>
        /// <param name="obj"> The natural object to draw </param>
        public void DrawObject(NaturalObject obj)
        {
            // Specify its type (tree or rock) and its size (small, medium or large).
            switch (obj.sizeType)
            {
                case SizeType.LARGE_TREE:
                    DrawSizeSpecificPixel(obj.x, obj.y, largeTree, objectSize.LARGE);
                    break;
                case SizeType.MEDIUM_TREE:
                    DrawSizeSpecificPixel(obj.x, obj.y, mediumTree, objectSize.MEDIUM);
                    break;
                case SizeType.SMALL_TREE:
                    DrawSizeSpecificPixel( obj.x, obj.y, smallTree, objectSize.SMALL);
                    break;
                case SizeType.LARGE_ROCK:
                    DrawSizeSpecificPixel(obj.x, obj.y, largeRock, objectSize.LARGE);
                    break;
                case SizeType.MEDIUM_ROCK:
                    DrawSizeSpecificPixel(obj.x, obj.y, mediumRock, objectSize.MEDIUM);
                    break;
                case SizeType.SMALL_ROCK:
                    DrawSizeSpecificPixel(obj.x, obj.y, smallRock, objectSize.SMALL);
                    break;
            }
        }

        /// <summary>
        /// Helper function which takes a size and a color and will draw the correct amount of pixels to the image.
        /// </summary>
        /// <param name="x"> The center x coordinate </param>
        /// <param name="y"> The center y coordinate </param>
        /// <param name="color"> The color of the natural object </param>
        /// <param name="size"> The size of the natural object </param>
        public void DrawSizeSpecificPixel(int x, int y, Color color, objectSize size)
        {
            newMap.SetPixel(x, y, color);

            if (size == objectSize.LARGE || size == objectSize.MEDIUM)
            {
                newMap.SetPixel(x, y - 1, color);
                newMap.SetPixel(x, y + 1, color);
                newMap.SetPixel(x - 1, y, color);
                newMap.SetPixel(x + 1, y, color);

                if (size == objectSize.LARGE)
                {
                    newMap.SetPixel(x - 1, y - 1, color);
                    newMap.SetPixel(x - 1, y + 1, color);
                    newMap.SetPixel(x + 1, y - 1, color);
                    newMap.SetPixel(x + 1, y + 1, color);
                }
            }
        }

        /// <summary>
        /// Saves the image to a specific file.
        /// </summary>
        /// <param name="name"> The name of the new image </param>
        public void Save(string name)
        {
            newMap.Save(name);
        }
    }
}
