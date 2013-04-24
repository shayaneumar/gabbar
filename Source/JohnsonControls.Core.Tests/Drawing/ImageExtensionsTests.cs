/*----------------------------------------------------------------------------

  (C) Copyright 2013 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnsonControls.Drawing
{
    [TestClass]
    public class ImageExtensionsTests
    {
        [TestMethod]
        public void DownScaleToEqualAspectRatio()
        {
            //arrange
            Image imageInput = new Bitmap(100, 100);

            //act
            Image imageOutput = imageInput.DownScaleTo(50, 50);

            //assert
            Assert.AreEqual(imageOutput.Width, 50);
            Assert.AreEqual(imageOutput.Height, 50);
        }

        [TestMethod]
        public void DownScaleToWidthSmaller()
        {
            //arrange
            Image imageInput = new Bitmap(100, 100);

            //act
            Image imageOutput = imageInput.DownScaleTo(60, 100);

            //assert
            Assert.AreEqual(imageOutput.Width, 60);
            Assert.AreEqual(imageOutput.Height, 60);
        }

        [TestMethod]
        public void DownScaleToHeightSmaller()
        {
            //arrange
            Image imageInput = new Bitmap(100, 100);

            //act
            Image imageOutput = imageInput.DownScaleTo(100, 40);

            //assert
            Assert.AreEqual(imageOutput.Width, 40);
            Assert.AreEqual(imageOutput.Height, 40);
        }

        [TestMethod]
        public void DownScaleToBigger()
        {
            //arrange
            Image imageInput = new Bitmap(100, 100);

            //act
            Image imageOutput = imageInput.DownScaleTo(200, 200);

            //assert
            Assert.AreEqual(imageOutput.Width, 100);
            Assert.AreEqual(imageOutput.Height, 100);
        }

        [TestMethod]
        public void ResizeTo()
        {
            //arrange
            Image imageInput = new Bitmap(100, 100);

            //act
            Image imageOutput = imageInput.ResizeTo(60, 40);

            //assert
            Assert.AreEqual(imageOutput.Width, 60);
            Assert.AreEqual(imageOutput.Height, 40);
        }
    }
}
