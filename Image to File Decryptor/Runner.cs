using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Image_to_File_Decryptor
{
    class Runner
    {
        // Define the new filenames.
        private static readonly string savefileName         = "PrototypeDataProcessed.txt";
        private static readonly string saveImageName        = "PrototypeImageProcessed.png";
        
        // Load the image in and save its width and height.
        private static Bitmap map = Resources.PrototypeWorld;
        public static int maxX = map.Width;
        public static int maxY = map.Height;

        // Define the colors which will be found in image.
        private static readonly Color grass          = Color.FromArgb(102, 153, 0);
        private static readonly Color forest         = Color.FromArgb(51, 102, 0);
        private static readonly Color deepWater      = Color.FromArgb(0, 102, 204);
        private static readonly Color shallowWater   = Color.FromArgb(51, 153, 255);
        
        // Create the lists of different grid types which trees will be spawned on.
        private static ArrayList grassPoints = new ArrayList();
        private static ArrayList forestPoints = new ArrayList();

        private static Dictionary<Color, int> colorDict;
        private static Dictionary<SizeType, int> objDict;

        private static ArrayList outputStrings;

     
        static void Main(string[] args)
        {
            BitmapEditor newMap = new BitmapEditor(map.Width, map.Height);
            PoissonSampler poissonSampler = new PoissonSampler();

            // Create the dictionary and load it with the color values.
            colorDict = new Dictionary<Color, int>
            {
                { grass, 0 },
                { forest, 1 },
                { shallowWater, 2 },
                { deepWater, 3 }
            };

            // Create the dictionary and load it with the obj values.
            objDict = new Dictionary<SizeType, int>
            {
                { SizeType.LARGE_TREE, 0 },
                { SizeType.MEDIUM_TREE, 1 },
                { SizeType.SMALL_TREE, 2 },
                { SizeType.LARGE_ROCK, 3 },
                { SizeType.MEDIUM_ROCK, 4 },
                { SizeType.SMALL_ROCK, 5 }
            };

            // Initialise the image parser variables.
            outputStrings = new ArrayList();

            ImageParser();

            // Initialise the PoissonDiscSampling variables.
            ArrayList naturalObjectsInGrassArea = new ArrayList();
            ArrayList naturalObjectsInForestArea = new ArrayList();
            Random rand = new Random();

            naturalObjectsInGrassArea = poissonSampler.BuildObjectList(grassPoints, 3, false);
            naturalObjectsInForestArea = poissonSampler.BuildObjectList(forestPoints, 20, true, naturalObjectsInGrassArea);

            // Now need to append the natural object data to the ends of the correct output strings in the array.
            foreach (NaturalObject obj in naturalObjectsInGrassArea)
            {
                int outputStringListIndex = (obj.x * (maxY)) + obj.y;
                outputStrings[outputStringListIndex] = outputStrings[outputStringListIndex] + "," + objDict[obj.sizeType].ToString();
                newMap.DrawObject(obj);
            }
            foreach (NaturalObject obj in naturalObjectsInForestArea)
            {
                int outputStringListIndex = (obj.x * (maxY)) + obj.y;
                outputStrings[outputStringListIndex] = outputStrings[outputStringListIndex] + "," + objDict[obj.sizeType].ToString();
                newMap.DrawObject(obj);
            }

            // Save the image and file data.
            SaveCalculatedDataToFile();
            newMap.Save(saveImageName);

            Console.WriteLine("DONE");
            Console.ReadLine();

        }

        /// <summary>
        /// Method to loop through the image which has been specified at the top of the file.
        /// While looping the method saves all the points and saves the relevant grass and forest values for Poisson Sampling.
        /// </summary>
        private static void ImageParser()
        {
            // Loop through all the pixels in the image.
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    // Get the current pixels color.
                    Color pixelColor = map.GetPixel(x, y);

                    // Calculate which cluster the pixel falls into, where each cluster contains 100 x 100 blocks.
                    int cluster = (x / 100) + ((y / 100) * (maxX / 100));

                    string outputString = x + "," + y + "," + cluster + ",";

                    if (pixelColor == grass)
                    {
                        outputString += colorDict[grass];
                        grassPoints.Add(new Point(x, y));
                    }
                    else if (pixelColor == forest)
                    {
                        outputString += colorDict[forest];
                        forestPoints.Add(new Point(x, y));
                    }

                    else if (pixelColor == deepWater) outputString += colorDict[deepWater];
                    else if (pixelColor == shallowWater) outputString += colorDict[shallowWater];

                    // Add the output string to list of out strings.
                    outputStrings.Add(outputString);
                }
            }
        }

        /// <summary>
        /// Method which will loop through the output strings and save the data to the specified file.
        /// </summary>
        private static void SaveCalculatedDataToFile()
        {
            foreach (string outputString in outputStrings)
            {
                // Write to the specificied file.
                using (StreamWriter sw = new StreamWriter(savefileName, true))
                {
                    Console.WriteLine(outputString);
                    sw.WriteLine(outputString);
                }
            }
        }
    }
}
