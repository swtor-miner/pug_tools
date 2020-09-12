/*
* Copyright (c) 2012 Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Runtime.InteropServices;

namespace DevIL.Unmanaged
{
    public static class ILU
    {

        private const string ILUDLL = "ILU.dll";
        private static bool _init = false;

        public static bool IsInitialized
        {
            get
            {
                return _init;
            }
        }

        #region ILU Methods

        public static void Initialize()
        {
            if (!_init)
            {
                IluInit();
                _init = true;
            }
        }

        [DllImport(ILUDLL, EntryPoint = "iluAlienify", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool Alienify();

        public static bool BlurAverage(int iterations)
        {
            return IluBlurAverage((uint)iterations);
        }

        public static bool BlurGaussian(int iterations)
        {
            return IluBlurGaussian((uint)iterations);
        }

        public static bool CompareImages(int otherImageID)
        {
            return IluCompareImages((uint)otherImageID);
        }

        public static bool Crop(int xOffset, int yOffset, int zOffset, int width, int height, int depth)
        {
            return IluCrop((uint)xOffset, (uint)yOffset, (uint)zOffset, (uint)width, (uint)height, (uint)depth);
        }

        [DllImport(ILUDLL, EntryPoint = "iluContrast", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool Contrast(float contrast);

        [DllImport(ILUDLL, EntryPoint = "iluEdgeDetectE", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool EdgeDetectE();

        [DllImport(ILUDLL, EntryPoint = "iluEdgeDetectP", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool EdgeDetectP();

        [DllImport(ILUDLL, EntryPoint = "iluEdgeDetectS", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool EdgeDetectS();

        [DllImport(ILUDLL, EntryPoint = "iluEmboss", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool Emboss();

        public static bool EnlargeCanvas(int width, int height, int depth)
        {
            return IluEnlargeCanvas((uint)width, (uint)height, (uint)depth);
        }

        public static bool EnlargeImage(int xDimension, int yDimension, int zDimension)
        {
            return IluEnlargeImage((uint)xDimension, (uint)yDimension, (uint)zDimension);
        }

        [DllImport(ILUDLL, EntryPoint = "iluEqualize", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool Equalize();

        public static string GetErrorString(ErrorType error)
        {
            //DevIL re-uses its error strings
            return Marshal.PtrToStringAnsi(IluGetErrorString((uint)error));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix">3x3 Matrix (row major)</param>
        /// <param name="scale"></param>
        /// <param name="bias"></param>
        /// <returns></returns>
        [DllImport(ILUDLL, EntryPoint = "iluConvolution", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool Convolution(int[] matrix, int scale, int bias);

        [DllImport(ILUDLL, EntryPoint = "iluFlipImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool FlipImage();

        [DllImport(ILUDLL, EntryPoint = "iluBuildMipmaps", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool BuildMipMaps();

        public static int ColorsUsed()
        {
            return (int)IluColorsUsed();
        }

        public static bool Scale(int width, int height, int depth)
        {
            return IluScale((uint)width, (uint)height, (uint)depth);
        }

        [DllImport(ILUDLL, EntryPoint = "iluGammaCorrect", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool GammaCorrect(float gamma);

        [DllImport(ILUDLL, EntryPoint = "iluInvertAlpha", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool InvertAlpha();

        [DllImport(ILUDLL, EntryPoint = "iluMirror", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool Mirror();

        [DllImport(ILUDLL, EntryPoint = "iluNegative", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool Negative();

        [DllImport(ILUDLL, EntryPoint = "iluNoisify", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IluNoisify(float tolerance);

        public static bool Noisify(float tolerance)
        {
            return IluNoisify(MemoryHelper.Clamp(tolerance, 0f, 1f));
        }

        public static bool Pixelize(int pixelSize)
        {
            return IluPixelize((uint)pixelSize);
        }

        [DllImport(ILUDLL, EntryPoint = "iluReplaceColour", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ReplaceColor(byte red, byte green, byte blue, float tolerance);

        [DllImport(ILUDLL, EntryPoint = "iluRotate", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool Rotate(float angle);

        [DllImport(ILUDLL, EntryPoint = "iluRotate3D", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool Rotate3D(float x, float y, float z, float angle);

        [DllImport(ILUDLL, EntryPoint = "iluSaturate1f", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool Saturate(float saturation);

        [DllImport(ILUDLL, EntryPoint = "iluSaturate4f", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool Saturate(float red, float green, float blue, float saturation);

        [DllImport(ILUDLL, EntryPoint = "iluScaleAlpha", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ScaleAlpha(float scale);

        [DllImport(ILUDLL, EntryPoint = "iluScaleColours", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ScaleColors(float red, float green, float blue);

        [DllImport(ILUDLL, EntryPoint = "iluSetLanguage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IluSetLanguage(uint language);

        public static bool SetLanguage(Language lang)
        {
            return IluSetLanguage((uint)lang);
        }

        public static bool Sharpen(float factor, int iterations)
        {
            return IluSharpen(factor, (uint)iterations);
        }

        [DllImport(ILUDLL, EntryPoint = "iluSwapColours", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool SwapColors();

        [DllImport(ILUDLL, EntryPoint = "iluWave", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool Wave(float angle);

        public static string GetVendorName()
        {
            return Marshal.PtrToStringAnsi(IluGetString(ILDefines.IL_VENDOR));
        }

        public static string GetVersionNumber()
        {
            return Marshal.PtrToStringAnsi(IluGetString(ILDefines.IL_VERSION_NUM));
        }

        public static void SetImagePlacement(Placement placement)
        {
            IluImageParameter(ILUDefines.ILU_PLACEMENT, (uint)placement);
        }

        public static Placement GetImagePlacement()
        {
            return (Placement)IluGetInteger(ILUDefines.ILU_PLACEMENT);
        }

        public static void SetSamplingFilter(SamplingFilter filter)
        {
            IluImageParameter(ILUDefines.ILU_FILTER, (uint)filter);
        }

        public static SamplingFilter GetSamplingFilter()
        {
            return (SamplingFilter)IluGetInteger(ILUDefines.ILU_FILTER);
        }

        public static void Region(PointF[] points)
        {
            if (points == null || points.Length < 3)
            {
                return;
            }
            IluRegionf(points, (uint)points.Length);
        }

        public static void Region(PointI[] points)
        {
            if (points == null || points.Length < 3)
            {
                return;
            }
            IluRegioni(points, (uint)points.Length);
        }

        #endregion

        #region ILU Native Methods

        [DllImport(ILUDLL, EntryPoint = "iluInit", CallingConvention = CallingConvention.StdCall)]
        private static extern void IluInit();

        [DllImport(ILUDLL, EntryPoint = "iluBlurAvg", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IluBlurAverage(uint iterations);

        [DllImport(ILUDLL, EntryPoint = "iluBlurGaussian", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IluBlurGaussian(uint iterations);

        [DllImport(ILUDLL, EntryPoint = "iluCompareImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IluCompareImages(uint otherImage);

        [DllImport(ILUDLL, EntryPoint = "iluCrop", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IluCrop(uint offsetX, uint offsetY, uint offsetZ, uint width, uint height, uint depth);

        [DllImport(ILUDLL, EntryPoint = "iluEnlargeCanvas", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IluEnlargeCanvas(uint width, uint height, uint depth);

        [DllImport(ILUDLL, EntryPoint = "iluEnlargeImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IluEnlargeImage(uint xDim, uint yDim, uint zDim);

        [DllImport(ILUDLL, EntryPoint = "iluErrorString", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr IluGetErrorString(uint error);

        [DllImport(ILUDLL, EntryPoint = "iluColoursUsed", CallingConvention = CallingConvention.StdCall)]
        private static extern uint IluColorsUsed();

        [DllImport(ILUDLL, EntryPoint = "iluScale", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IluScale(uint width, uint height, uint depth);

        [DllImport(ILUDLL, EntryPoint = "iluPixelize", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IluPixelize(uint pixelSize);

        [DllImport(ILUDLL, EntryPoint = "iluSharpen", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IluSharpen(float factor, uint iterations);

        [DllImport(ILUDLL, EntryPoint = "iluGetString", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr IluGetString(uint name);

        [DllImport(ILUDLL, EntryPoint = "iluImageParameter", CallingConvention = CallingConvention.StdCall)]
        private static extern void IluImageParameter(uint pName, uint param);

        [DllImport(ILUDLL, EntryPoint = "iluGetInteger", CallingConvention = CallingConvention.StdCall)]
        private static extern int IluGetInteger(uint mode);

        [DllImport(ILUDLL, EntryPoint = "iluRegionfv", CallingConvention = CallingConvention.StdCall)]
        private static extern void IluRegionf(PointF[] points, uint num);

        [DllImport(ILUDLL, EntryPoint = "iluRegioniv", CallingConvention = CallingConvention.StdCall)]
        private static extern void IluRegioni(PointI[] points, uint num);

        #endregion
    }
}
