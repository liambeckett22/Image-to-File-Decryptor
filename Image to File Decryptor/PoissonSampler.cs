using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace Image_to_File_Decryptor
{
    class PoissonSampler
    {
        // Want 70% Large, 20% Medium and 10% Small trees spawned so will use random indexs and the below list to achive this.
        static readonly IList<objectSize> SizesPropability = new ReadOnlyCollection<objectSize>
        (new List<objectSize> { objectSize.LARGE, objectSize.LARGE, objectSize.LARGE, objectSize.LARGE, objectSize.LARGE,
                objectSize.LARGE, objectSize.LARGE, objectSize.MEDIUM, objectSize.MEDIUM, objectSize.SMALL });

        private ArrayList smallObjectsAcceptablePoints;
        private ArrayList largeMediumAcceptablePoints;

        private Random random;

        public PoissonSampler()
        {
            random = new Random();
        }

        /// <summary>
        /// Generates an array of natural objects which have been randomly distributed using Poission Disc Sampling. Will take into 
        /// account the density of the area.
        /// </summary>
        /// <param name="acceptablePoints"> The list of the points which objects can be placed in </param>
        /// <param name="maxAttempts"> How many times the program should attempt to place an object </param>
        /// <param name="isForest"> Controls the density and distribution of object, a forest will be densier and have a larger tree 
        /// to rock ratio </param>
        /// <param name="otherListToCheck"> Optional: Another list of objects which need to be checked to ensure no overlap </param>
        /// <returns></returns>
        public ArrayList BuildObjectList(ArrayList acceptablePoints, int maxAttempts, bool isForest, ArrayList otherListToCheck = null)
        {
            ArrayList result = new ArrayList();

            smallObjectsAcceptablePoints = acceptablePoints;

            // Need to create and fill a second arraylist which removes the outer ring of points as large and medium
            // objects will go out of bounds if placed here.
            largeMediumAcceptablePoints = new ArrayList();
            foreach (Point point in acceptablePoints)
            {
                if(!(point.X <= 0 || point.X >= Runner.maxX - 1 || point.Y <= 0 || point.Y >= Runner.maxY - 1))
                {
                    largeMediumAcceptablePoints.Add(point);
                }
            }

            // Add an intial object to the result.
            NaturalObject currentObject = GetRandomNaturalObject(isForest);
            result.Add(currentObject);
            
            // Keep randomly placing trees and checking for clashes, once x clashes have occured stop.
            bool done = false;
            while (!done)
            {
                int attemptCount = 0;
                bool placedObject = false;
                NaturalObject nextObject = null;

                while (attemptCount < maxAttempts && !placedObject)
                {
                    attemptCount++;
                    nextObject = GetRandomNaturalObject(isForest);
                    if (!TooClose(nextObject, result, otherListToCheck))
                    {
                        placedObject = true;
                    }
                }

                if (placedObject) result.Add(nextObject); 
                else done = true; 
            }
            return result;
        }

        /// <summary>
        /// Method to generate a random Object.
        /// </summary>
        /// <param name="isForest"> true if the object is for the forest, otherwise false </param>
        /// <returns></returns>
        private NaturalObject GetRandomNaturalObject(bool isForest)
        {
            objectSize size = GetRandomObjectSize();

            NaturalObject newObject;

            if (size == objectSize.SMALL) { newObject = new NaturalObject(random, size, smallObjectsAcceptablePoints, isForest); }
            else { newObject = new NaturalObject(random, size, largeMediumAcceptablePoints, isForest); }
        
            return newObject;
        }

        /// <summary>
        /// Creates a random number between 0 and 9 and uses it to get a values form the list of object Sizes
        /// </summary>
        /// <returns></returns>
        private objectSize GetRandomObjectSize()
        {
            objectSize treeType = SizesPropability[random.Next(0, 10)];

            return treeType;
        }

        /// <summary>
        /// Will check to see if a potenial object is too close to another object.
        /// </summary>
        /// <param name="nObject"> The object which needs to be checked </param>
        /// <param name="objectList1"> The current list of objects </param>
        /// <param name="objectList2"> Optional: Another list of object to check </param>
        /// <returns></returns>
        private bool TooClose(NaturalObject nObject, ArrayList objectList1, ArrayList objectList2 = null)
        {
            // Check the first list.
            foreach (NaturalObject otherTree in objectList1)
            {
                if (SingleTreeCheck(nObject, otherTree)) return true;
            }
            // Check the second list.
            if (objectList2 != null)
            {
                foreach (NaturalObject otherTree in objectList2)
                {
                    if (SingleTreeCheck(nObject, otherTree)) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Method which specifically checks to see if two objects overlap.
        /// </summary>
        /// <param name="nObject"> The first NaturalObject </param>
        /// <param name="otherNObject"> The second NaturalObject </param>
        /// <returns> True if the objects overlap </returns>
        private bool SingleTreeCheck(NaturalObject nObject, NaturalObject otherNObject)
        {
            float xDiff = nObject.x - otherNObject.x;
            float zDiff = nObject.y - otherNObject.y;

            float dist = (float)Math.Sqrt((xDiff * xDiff) + (zDiff * zDiff));
            if (dist < nObject.r + otherNObject.r)
            {
                return true;
            }
            return false;
        }
    }
}
