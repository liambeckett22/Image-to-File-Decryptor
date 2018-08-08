using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace Image_to_File_Decryptor
{
    /// <summary>
    /// This can be a tree or a rock object and holds all the relevant information for them.
    /// </summary>
    public class NaturalObject
    {
        public int x;
        public int y;
        public float r;
        public objectSize size;
        public objectType type;
        public SizeType sizeType;
        
        const int cellSize = 1;
        const float smallTreeRadius = cellSize / 2.0f;
        const float largeMediumTreeRadius = (cellSize * 3) / 2.0f;

        static readonly IList<objectType> ForestTypePropability = new ReadOnlyCollection<objectType>
            (new List<objectType> { objectType.TREE, objectType.TREE, objectType.TREE, objectType.TREE, objectType.ROCK });

        static readonly IList<objectType> GrassTypePropability = new ReadOnlyCollection<objectType>
           (new List<objectType> { objectType.TREE, objectType.TREE, objectType.ROCK, objectType.ROCK });
        
        /// <summary>
        /// Constructor which takes the treeSize(radius) and the list of acceptable points in the X and Z space.These are the grid corrdinates
        /// which the trees can spawn into.
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="size"></param>
        /// <param name="acceptablePoints"></param>
        /// <param name="isForest"></param>
        public NaturalObject(Random rng, objectSize size, ArrayList acceptablePoints, bool isForest)
        {
            this.size = size;

            if (isForest) type = ForestTypePropability[rng.Next(0, ForestTypePropability.Count)];
            else type = GrassTypePropability[rng.Next(0, GrassTypePropability.Count)];

            if (size == objectSize.LARGE || size == objectSize.MEDIUM)
            {
                // Large trees are 3x3 so the outer ring of the grid cannot be used.
                Point point = (Point)acceptablePoints[rng.Next(0, acceptablePoints.Count)];
                
                x = point.X;
                y = point.Y;
                r = largeMediumTreeRadius;
            }
            else
            {
                // Small trees are 1x1 so the outer ring of the grid can be used.
                Point point = (Point)acceptablePoints[rng.Next(0, acceptablePoints.Count)];
                x = point.X;
                y = point.Y;
                r = smallTreeRadius;
            }

            SetSizeType();
        }
        
        /// <summary>
        /// Sets the Size Type variable for the object.
        /// </summary>
        public void SetSizeType()
        {
            if (size == objectSize.LARGE)
            {
                if (type == objectType.TREE) sizeType = SizeType.LARGE_TREE;
                else sizeType = SizeType.LARGE_ROCK;
            }
            else if (size == objectSize.MEDIUM)
            {
                if (type == objectType.TREE) sizeType = SizeType.MEDIUM_TREE;
                else sizeType = SizeType.MEDIUM_ROCK;
            }
            else
            {
                if (type == objectType.TREE) sizeType = SizeType.SMALL_TREE;
                else sizeType = SizeType.SMALL_ROCK;
            }
        }
    }
}
