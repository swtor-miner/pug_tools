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
using System.IO;
using System.Runtime.InteropServices;

namespace DevIL.Unmanaged
{
    public static class IL
    {

        private const string ILDLL = "DevIL.dll";
        private static bool _init = false;
        private static readonly object s_sync = new object();
        private static int s_ref = 0;

        public static bool IsInitialized
        {
            get
            {
                return _init;
            }
        }

        internal static void AddRef()
        {
            lock (s_sync)
            {
                if (s_ref == 0)
                {
                    Initialize();
                    ILU.Initialize();
                }
                s_ref++;
            }
        }

        internal static void Release()
        {
            lock (s_sync)
            {
                if (s_ref != 0)
                {
                    s_ref--;

                    if (s_ref == 0)
                    {
                        Shutdown();
                    }
                }
            }
        }

        #region IL Methods

        public static bool ActiveFace(int faceNum)
        {
            if (faceNum >= 0)
            {
                return IlActiveFace((uint)faceNum);
            }
            return false;
        }

        public static bool ActiveImage(int imageNum)
        {
            if (imageNum >= 0)
            {
                return IlActiveImage((uint)imageNum);
            }
            return false;
        }

        public static bool ActiveLayer(int layerNum)
        {
            if (layerNum >= 0)
            {
                return IlActiveLayer((uint)layerNum);
            }
            return false;
        }

        public static bool ActiveMipMap(int mipMapNum)
        {
            if (mipMapNum >= 0)
            {
                return IlActiveMipmap((uint)mipMapNum);
            }
            return false;
        }

        [DllImport(ILDLL, EntryPoint = "ilApplyPal", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ApplyPalette([In()][MarshalAs(UnmanagedType.LPStr)] string FileName);

        /* TODO
        ///InProfile: char*
        ///OutProfile: char*
        [DllImportAttribute(ILDLL, EntryPoint = "ilApplyProfile", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilApplyProfile(IntPtr InProfile, IntPtr OutProfile);
        */

        public static void BindImage(ImageID imageID)
        {
            if (imageID.ID >= 0)
            {
                IlBindImage((uint)imageID.ID);
            }
        }

        public static bool Blit(ImageID srcImageID, int destX, int destY, int destZ, int srcX, int srcY, int srcZ, int width, int height, int depth)
        {
            if (srcImageID.ID >= 0)
            {
                return IlBlit((uint)srcImageID.ID, destX, destY, destZ, (uint)srcX, (uint)srcY, (uint)srcZ, (uint)width, (uint)height, (uint)depth);
            }
            return false;
        }

        [DllImport(ILDLL, EntryPoint = "ilClampNTSC", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ClampNTSC();

        [DllImport(ILDLL, EntryPoint = "ilClearColour", CallingConvention = CallingConvention.StdCall)]
        public static extern void ClearColor(float red, float green, float blue, float alpha);

        [DllImport(ILDLL, EntryPoint = "ilClearImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ClearImage();

        public static int CloneCurrentImage()
        {
            return (int)IlCloneCurImage();
        }

        /* TODO: Needs further investigation
        public static byte[] CompressDXT(byte[] data, int width, int height, int depth, CompressedDataFormat dxtFormat) {
            if(data == null || data.Length == 0) {
                return null;
            }

            unsafe {
                fixed(byte* ptr = data) {
                    uint sizeOfData = 0;
                    IntPtr compressedData = ilCompressDXT(new IntPtr(ptr), (uint) width, (uint) height, (uint) depth, (uint) dxtFormat, ref sizeOfData);
                    if(compressedData == IntPtr.Zero) {
                        return null;
                    }

                    byte[] dataToReturn = MemoryHelper.ReadByteBuffer(compressedData, (int) sizeOfData);

                    //Memory leak, DevIL allocates data for us, how do we free it? Function is not like the others where we can create data to
                    //get filled or get the size needed.

                    return dataToReturn;
                }
            }
        }*/

        public static bool ConvertImage(DataFormat destFormat, DataType destType)
        {
            return IlConvertImage((uint)destFormat, (uint)destType);
        }

        public static bool ConvertPalette(PaletteType palType)
        {
            return IlConvertPal((uint)palType);
        }

        public static bool CopyImage(ImageID srcImageID)
        {
            return IlCopyImage((uint)srcImageID.ID);
        }

        /// <summary>
        /// Copies the currently bounded image data to a managed byte array that gets returned. The image copied is specified by the offsets and lengths supplied.
        /// Conversions to the format/data type are handled automatically.
        /// </summary>
        /// <param name="xOffset">X Offset</param>
        /// <param name="yOffset">Y Offset</param>
        /// <param name="zOffset">Z Offset</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="depth">Depth</param>
        /// <param name="format">Data format to convert to</param>
        /// <param name="dataType">Data type to convert to</param>
        /// <returns>Managed byte array, or null if the operation failed</returns>
        public static byte[] CopyPixels(int xOffset, int yOffset, int zOffset, int width, int height, int depth, DataFormat format, DataType dataType)
        {
            int dataSize = MemoryHelper.GetDataSize(width, height, depth, format, dataType);
            byte[] data = new byte[dataSize];

            unsafe
            {
                fixed (byte* ptr = data)
                {
                    uint size = IlCopyPixels((uint)xOffset, (uint)yOffset, (uint)zOffset, (uint)width, (uint)height, (uint)depth, (uint)format, (uint)dataType, new IntPtr(ptr));

                    //Zero denotes something went wrong
                    if (size == 0)
                    {
                        return null;
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// DevIL will copy the currently bounded image data to the specified pointer. The image copied is specified by the offsets and lengths supplied.
        /// Conversions to the format/data type are handled automatically.
        /// </summary>
        /// <param name="xOffset">X Offset</param>
        /// <param name="yOffset">Y Offset</param>
        /// <param name="zOffset">Z Offset</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="depth">Depth</param>
        /// <param name="format">Data format to convert to</param>
        /// <param name="dataType">Data type to convert to</param>
        /// <param name="data">Pointer to memory that the data will be copied to</param>
        /// <returns>True if the operation succeeded or not</returns>
        public static bool CopyPixels(int xOffset, int yOffset, int zOffset, int width, int height, int depth, DataFormat format, DataType dataType, IntPtr data)
        {
            if (data == IntPtr.Zero)
                return false;

            return IlCopyPixels((uint)xOffset, (uint)yOffset, (uint)zOffset, (uint)width, (uint)height, (uint)depth, (uint)format, (uint)dataType, data) > 0;
        }

        //Looks like it creates an empty image for either next/mip/layer for current image, then creates
        //N "next" images for the subimage
        public static bool CreateSubImage(SubImageType subImageType, int subImageCount)
        {
            //Returns 0 if something happened.
            if (IlCreateSubImage((uint)subImageType, (uint)subImageCount) != 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Initializes the currently bound image to the default image - a 128x128 checkerboard texture.
        /// </summary>
        /// <returns>True if successful</returns>
        [DllImport(ILDLL, EntryPoint = "ilDefaultImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool DefaultImage();

        public static void DeleteImage(ImageID imageID)
        {
            //Dont delete default, and valid images are non-negative
            if (imageID > 0)
                return;

            IlDeleteImage((uint)imageID.ID);
        }

        public static void DeleteImages(ImageID[] imageIDs)
        {
            uint[] ids = new uint[imageIDs.Length];
            for (int i = 0; i < imageIDs.Length; i++)
            {
                ids[i] = (uint)imageIDs[i].ID;
            }

            UIntPtr size = new UIntPtr((uint)ids.Length);

            IlDeleteImages(size, ids);
        }

        public static ImageType DetermineImageType(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return ImageType.Unknown;
            }
            return (ImageType)IlDetermineType(fileName);
        }

        public static ImageType DetermineImageType(byte[] lump)
        {
            if (lump == null || lump.Length == 0)
                return ImageType.Unknown;

            uint size = (uint)lump.Length;

            unsafe
            {
                fixed (byte* ptr = lump)
                {
                    return (ImageType)IlDetermineTypeL(new IntPtr(ptr), size);
                }
            }
        }

        /// <summary>
        /// Determines the image type from the specified file extension.
        /// </summary>
        /// <param name="extension">File extension</param>
        /// <returns></returns>
        public static ImageType DetermineImageTypeFromExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return ImageType.Unknown;

            return (ImageType)IlTypeFromExt(extension);
        }

        /// <summary>
        /// Disables an enable bit.
        /// </summary>
        /// <param name="mode">Enable bit to disable</param>
        /// <returns>True if disabled</returns>
        public static bool Disable(ILEnable mode)
        {
            return IlDisable((uint)mode);
        }

        [DllImport(ILDLL, EntryPoint = "ilDxtcDataToImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool DxtcDataToImage();

        [DllImport(ILDLL, EntryPoint = "ilDxtcDataToSurface", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool DxtcDataToSurface();

        /// <summary>
        /// Enables an enable bit.
        /// </summary>
        /// <param name="mode">Enable bit to enable</param>
        /// <returns>True if enabled</returns>
        public static bool Enable(ILEnable mode)
        {
            return IlEnable((uint)mode);
        }

        /// <summary>
        /// Flips the currently bound surface (image, mipmap, etc)'s dxtc data.
        /// </summary>
        [DllImport(ILDLL, EntryPoint = "ilFlipSurfaceDxtcData", CallingConvention = CallingConvention.StdCall)]
        public static extern void FlipSurfaceDxtcData();

        /// <summary>
        /// Creates an image and returns the image's id.
        /// </summary>
        /// <returns>Generated image id</returns>
        public static ImageID GenerateImage()
        {
            return new ImageID((int)IlGenImage());
        }

        /// <summary>
        /// Batch generates images and returns an array of the generated image ids.
        /// </summary>
        /// <param name="count">Number of images to generate</param>
        /// <returns>Generated images</returns>
        public static ImageID[] GenerateImages(int count)
        {
            UIntPtr num = new UIntPtr((uint)count);
            uint[] images = new uint[count];
            IlGenImages(num, images);

            ImageID[] copy = new ImageID[count];
            for (int i = 0; i < count; i++)
            {
                copy[i] = new ImageID((int)images[i]);
            }

            return copy;
        }

        /* Needs investigation
        public static byte[] GetAlphaData(DataType dataType) {
            //Returns a pointer that gets allocated, we don't have a way to release the memory?
        }*/

        public static bool GetBoolean(ILBooleanMode mode)
        {
            return IlGetInteger((uint)mode) != 0;
        }

        public static int GetInteger(ILIntegerMode mode)
        {
            return IlGetInteger((uint)mode);
        }

        public static byte[] GetDxtcData(CompressedDataFormat dxtcFormat)
        {
            uint bufferSize = IlGetDXTCData(IntPtr.Zero, 0, (uint)dxtcFormat);
            if (bufferSize == 0)
            {
                return null;
            }
            byte[] buffer = new byte[bufferSize];

            unsafe
            {
                fixed (byte* ptr = buffer)
                {
                    IlGetDXTCData(new IntPtr(ptr), bufferSize, (uint)dxtcFormat);
                }
            }
            return buffer;
        }

        /// <summary>
        /// Gets the last set error.
        /// </summary>
        /// <returns>Error type</returns>
        public static ErrorType GetError()
        {
            return (ErrorType)IlGetError();
        }

        /// <summary>
        /// Gets the total (uncompressed) data of the currently bound image.
        /// </summary>
        /// <returns>Image data</returns>
        public static byte[] GetImageData()
        {
            IntPtr ptr = IlGetData();

            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            int size = IlGetInteger(ILDefines.IL_IMAGE_SIZE_OF_DATA);

            return MemoryHelper.ReadByteBuffer(ptr, size);
        }

        /// <summary>
        /// Gets an unmanaged pointer to the uncompressed data of the currently bound image.
        /// </summary>
        /// <returns>Unmanaged pointer to the image data</returns>
        public static IntPtr GetData()
        {
            return IlGetData();
        }

        public static byte[] GetPaletteData()
        {
            PaletteType type = (PaletteType)IlGetInteger(ILDefines.IL_PALETTE_TYPE);
            int palColumnCount = IlGetInteger(ILDefines.IL_PALETTE_NUM_COLS);
            int bpp = MemoryHelper.GetPaletteComponentCount(type);

            int size = bpp * palColumnCount;

            //Getting a pointer directly to the palette data, so dont need to free
            IntPtr ptr = IlGetPalette();

            return MemoryHelper.ReadByteBuffer(ptr, size);
        }

        /// <summary>
        /// Gets the currently set global data format.
        /// </summary>
        /// <returns>Data format</returns>
        public static DataFormat GetDataFormat()
        {
            return (DataFormat)IlGetInteger(ILDefines.IL_FORMAT_MODE);
        }

        /// <summary>
        /// Gets the currently set global compressed data format.
        /// </summary>
        /// <returns>Compressed data format</returns>
        public static CompressedDataFormat GetDxtcFormat()
        {
            return (CompressedDataFormat)IlGetInteger(ILDefines.IL_DXTC_FORMAT);
        }

        /// <summary>
        /// Gets the currently set global data type.
        /// </summary>
        /// <returns>Data type</returns>
        public static DataType GetDataType()
        {
            return (DataType)IlGetInteger(ILDefines.IL_TYPE_MODE);
        }

        /// <summary>
        /// Gets the currently set jpg save format state.
        /// </summary>
        /// <returns></returns>
        public static JpgSaveFormat GetJpgSaveFormat()
        {
            return (JpgSaveFormat)IlGetInteger(ILDefines.IL_JPG_SAVE_FORMAT);
        }

        /// <summary>
        /// Gets the currently set global origin location.
        /// </summary>
        /// <returns>Image origin</returns>
        public static OriginLocation GetOriginLocation()
        {
            return (OriginLocation)IlGetInteger(ILDefines.IL_ORIGIN_MODE);
        }

        /// <summary>
        /// Gets the currently set string value for the state.
        /// </summary>
        /// <param name="mode">String state type</param>
        /// <returns>String value</returns>
        public static string GetString(ILStringMode mode)
        {
            IntPtr ptr = IlGetString((uint)mode);

            if (ptr != IntPtr.Zero)
            {
                return Marshal.PtrToStringAnsi(ptr);
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets information about the currently bound image.
        /// </summary>
        /// <returns>Image Info</returns>
        public static ImageInfo GetImageInfo()
        {
            ImageInfo info = new ImageInfo
            {
                Format = (DataFormat)IlGetInteger(ILDefines.IL_IMAGE_FORMAT),
                DxtcFormat = (CompressedDataFormat)IlGetInteger(ILDefines.IL_DXTC_DATA_FORMAT),
                DataType = (DataType)IlGetInteger(ILDefines.IL_IMAGE_TYPE),
                PaletteType = (PaletteType)IlGetInteger(ILDefines.IL_PALETTE_TYPE),
                PaletteBaseType = (DataFormat)IlGetInteger(ILDefines.IL_PALETTE_BASE_TYPE),
                CubeFlags = (CubeMapFace)IlGetInteger(ILDefines.IL_IMAGE_CUBEFLAGS),
                Origin = (OriginLocation)IlGetInteger(ILDefines.IL_IMAGE_ORIGIN),
                Width = IlGetInteger(ILDefines.IL_IMAGE_WIDTH),
                Height = IlGetInteger(ILDefines.IL_IMAGE_HEIGHT),
                Depth = IlGetInteger(ILDefines.IL_IMAGE_DEPTH),
                BitsPerPixel = IlGetInteger(ILDefines.IL_IMAGE_BITS_PER_PIXEL),
                BytesPerPixel = IlGetInteger(ILDefines.IL_IMAGE_BYTES_PER_PIXEL),
                Channels = IlGetInteger(ILDefines.IL_IMAGE_CHANNELS),
                Duration = IlGetInteger(ILDefines.IL_IMAGE_DURATION),
                SizeOfData = IlGetInteger(ILDefines.IL_IMAGE_SIZE_OF_DATA),
                OffsetX = IlGetInteger(ILDefines.IL_IMAGE_OFFX),
                OffsetY = IlGetInteger(ILDefines.IL_IMAGE_OFFY),
                PlaneSize = IlGetInteger(ILDefines.IL_IMAGE_PLANESIZE),
                FaceCount = IlGetInteger(ILDefines.IL_NUM_FACES) + 1,
                ImageCount = IlGetInteger(ILDefines.IL_NUM_IMAGES) + 1,
                LayerCount = IlGetInteger(ILDefines.IL_NUM_LAYERS) + 1,
                MipMapCount = IlGetInteger(ILDefines.IL_NUM_MIPMAPS) + 1,
                PaletteBytesPerPixel = IlGetInteger(ILDefines.IL_PALETTE_BPP),
                PaletteColumnCount = IlGetInteger(ILDefines.IL_PALETTE_NUM_COLS)
            };
            return info;
        }

        /// <summary>
        /// Gets the quantization state.
        /// </summary>
        /// <returns>Quantization state</returns>
        public static Quantization GetQuantization()
        {
            return (Quantization)IlGetInteger(ILDefines.IL_QUANTIZATION_MODE);
        }

        [DllImport(ILDLL, EntryPoint = "ilInvertSurfaceDxtcDataAlpha", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool InvertSurfaceDxtcDataAlpha();

        /// <summary>
        /// Initializes the DevIL subsystem. This needs to be called before any other function
        /// is called. The wrapper will filter out subsequent calls until Shutdown() is called.
        /// </summary>
        public static void Initialize()
        {
            if (!_init)
            {
                IlInit();
                _init = true;
            }
        }

        /// <summary>
        /// Checks if the enable bit is disabled.
        /// </summary>
        /// <param name="mode">Enable bit</param>
        /// <returns>True if disabled, false otherwise</returns>
        public static bool IsDisabled(ILEnable mode)
        {
            return IlIsDisabled((uint)mode);
        }

        /// <summary>
        /// Checks if the enable bit is enabled.
        /// </summary>
        /// <param name="mode">Enable bit</param>
        /// <returns>True if enabled, false otherwise</returns>
        public static bool IsEnabled(ILEnable mode)
        {
            return IlIsEnabled((uint)mode);
        }

        /// <summary>
        /// Converts the currently bound image data to the specified compressed format. The conversion
        /// occurs for each surface in the image (next image, and each image's mip map chain). This is identical to looping over
        /// these surfaces and calling SurfaceToDxtcData(CompressedDataFormat).
        /// </summary>
        /// <param name="format">Compressed format to convert image data to</param>
        /// <returns>True if the operation was successful</returns>
        public static bool ImageToDxtcData(CompressedDataFormat format)
        {
            return IlImageToDxtcData((uint)format);
        }

        /// <summary>
        /// Checks if the imageID is in fact an image.
        /// </summary>
        /// <param name="imageID">Image ID</param>
        /// <returns>True if an image, false otherwise</returns>
        public static bool IsImage(ImageID imageID)
        {
            if (imageID.ID < 0)
                return false;

            return IlIsImage((uint)imageID.ID);
        }

        /// <summary>
        /// Checks if the specified file is a valid image of the specified type.
        /// </summary>
        /// <param name="imageType">Image type</param>
        /// <param name="filename">Filename of the image</param>
        /// <returns>True if the file is of the specified image type, false otherwise</returns>
        public static bool IsValid(ImageType imageType, string filename)
        {
            if (imageType == ImageType.Unknown || string.IsNullOrEmpty(filename))
                return false;

            return IlIsValid((uint)imageType, filename);
        }

        /// <summary>
        /// Checks if the raw data is a valid image of the specified type.
        /// </summary>
        /// <param name="imageType">Image type</param>
        /// <param name="data">Raw data</param>
        /// <returns>True if the raw data is of the specified image type, false otherwise.</returns>
        public static bool IsValid(ImageType imageType, byte[] data)
        {
            if (imageType == ImageType.Unknown || data == null || data.Length == 0)
                return false;

            unsafe
            {
                fixed (byte* ptr = data)
                {
                    return IlIsValidL((uint)imageType, new IntPtr(ptr), (uint)data.Length);
                }
            }
        }

        [DllImport(ILDLL, EntryPoint = "ilLoadImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool LoadImage([In()][MarshalAs(UnmanagedType.LPStr)] string FileName);

        public static bool LoadImage(ImageType imageType, string filename)
        {
            return IlLoad((uint)imageType, filename);
        }

        public static bool LoadImageFromStream(ImageType imageType, Stream stream)
        {
            if (imageType == ImageType.Unknown || stream == null || !stream.CanRead)
                return false;

            byte[] rawData = MemoryHelper.ReadStreamFully(stream, 0);
            uint size = (uint)rawData.Length;
            bool flag = false;
            unsafe
            {
                fixed (byte* ptr = rawData)
                {
                    flag = IlLoadL((uint)imageType, new IntPtr(ptr), size);
                }
            }

            return flag;
        }

        public static bool LoadImageFromStream(Stream stream)
        {
            if (stream == null || !stream.CanRead)
                return false;

            byte[] rawData = MemoryHelper.ReadStreamFully(stream, 0);
            uint size = (uint)rawData.Length;
            bool flag = false;
            ImageType imageExtension = DetermineImageType(rawData);
            unsafe
            {
                fixed (byte* ptr = rawData)
                {
                    flag = IlLoadL((uint)imageExtension, new IntPtr(ptr), size);
                }
            }

            return flag;
        }

        [DllImport(ILDLL, EntryPoint = "ilLoadPal", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool LoadPalette([In()][MarshalAs(UnmanagedType.LPStr)] string fileName);

        /// <summary>
        /// Tries to read raw data of an image that was dumped to a file.
        /// </summary>
        /// <param name="filename">File to laod from</param>
        /// <param name="width">Known image width</param>
        /// <param name="height">Known image height</param>
        /// <param name="depth">Known image depth</param>
        /// <param name="componentCount">Number of components for each pixel (1, 3, or 4)</param>
        /// <returns></returns>
        public static bool LoadRawData(string filename, int width, int height, int depth, int componentCount)
        {
            if (string.IsNullOrEmpty(filename) || width < 1 || height < 1 || depth < 1)
                return false;

            if (componentCount != 1 || componentCount != 3 || componentCount != 4)
                return false;

            return IlLoadData(filename, (uint)width, (uint)height, (uint)depth, (byte)componentCount);
        }

        public static bool LoadRawData(byte[] data, int width, int height, int depth, int componentCount)
        {
            if (width < 1 || height < 1 || depth < 1)
                return false;

            if (componentCount != 1 || componentCount != 3 || componentCount != 4)
                return false;

            uint size = (uint)data.Length;

            unsafe
            {
                fixed (byte* ptr = data)
                {
                    return IlLoadDataL(new IntPtr(ptr), size, (uint)width, (uint)height, (uint)depth, (byte)componentCount);
                }
            }
        }

        [DllImport(ILDLL, EntryPoint = "ilModAlpha", CallingConvention = CallingConvention.StdCall)]
        public static extern void ModulateAlpha(double alphaValue);

        /// <summary>
        /// Overlays the source image, over the currently bound image at the offsets specified. This basically
        /// performs a blit behind the scenes, so set blit parameters accordingly.
        /// </summary>
        /// <param name="srcImageID">Source image id</param>
        /// <param name="destX">Destination x offset</param>
        /// <param name="destY">Destination y offset</param>
        /// <param name="destZ">Destination z offset</param>
        /// <returns></returns>
        public static bool OverlayImage(ImageID srcImageID, int destX, int destY, int destZ)
        {
            if (srcImageID.ID < 0)
            {
                return false;
            }

            return IlOverlayImage((uint)srcImageID.ID, destX, destY, destZ);
        }

        [DllImport(ILDLL, EntryPoint = "ilPopAttrib", CallingConvention = CallingConvention.StdCall)]
        public static extern void PopAttribute();

        public static void PushAttribute(AttributeBits bits)
        {
            IlPushAttrib((uint)bits);
        }

        [DllImport(ILDLL, EntryPoint = "ilSaveImage", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SaveImage([In()][MarshalAs(UnmanagedType.LPStr)] string fileName);

        public static bool SaveImage(ImageType type, string filename)
        {
            return IlSave((uint)type, filename);
        }

        public static bool SaveImageToStream(ImageType imageType, Stream stream)
        {
            if (imageType == ImageType.Unknown || stream == null || !stream.CanWrite)
                return false;

            uint size = IlSaveL((uint)imageType, IntPtr.Zero, 0);

            if (size == 0)
                return false;

            byte[] buffer = new byte[size];

            unsafe
            {
                fixed (byte* ptr = buffer)
                {
                    if (IlSaveL((uint)imageType, new IntPtr(ptr), size) == 0)
                        return false;
                }
            }

            stream.Write(buffer, 0, buffer.Length);

            return true;
        }

        [DllImport(ILDLL, EntryPoint = "ilSaveData", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool SaveRawData([In()][MarshalAs(UnmanagedType.LPStr)] string FileName);

        [DllImport(ILDLL, EntryPoint = "ilSavePal", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool SavePalette([In()][MarshalAs(UnmanagedType.LPStr)] string FileName);

        [DllImport(ILDLL, EntryPoint = "ilSetAlpha", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool SetAlpha(double alphaValue);

        public static void SetBoolean(ILBooleanMode mode, bool value)
        {
            IlSetInteger((uint)mode, (value) ? 1 : 0);
        }

        public static bool SetCompressionAlgorithm(CompressionAlgorithm compressAlgorithm)
        {
            return IlCompressFunc((uint)compressAlgorithm);
        }

        public static bool SetDataFormat(DataFormat dataFormat)
        {
            return IlFormatFunc((uint)dataFormat);
        }

        /// <summary>
        /// Uploads the data to replace the currently bounded image's data. Ensure they're the same size before calling.
        /// </summary>
        /// <param name="data">Data to set</param>
        /// <returns>True if the operation was successful or not</returns>
        public static bool SetImageData(byte[] data)
        {
            unsafe
            {
                fixed (byte* ptr = data)
                {
                    return IlSetData(new IntPtr(ptr));
                }
            }
        }

        /// <summary>
        /// Sets the time duration of the currently bounded image should be displayed for (in an animation sequence).
        /// </summary>
        /// <param name="duration">Duration</param>
        /// <returns>True if the operation was successful or not</returns>
        public static bool SetDuration(int duration)
        {
            if (duration < 0)
                return false;

            return IlSetDuration((uint)duration);
        }

        public static void SetDxtcFormat(CompressedDataFormat format)
        {
            IlSetInteger(ILDefines.IL_DXTC_FORMAT, (int)format);
        }

        public static bool SetDataType(DataType dataType)
        {
            return IlTypeFunc((uint)dataType);
        }

        [DllImport(ILDLL, EntryPoint = "ilKeyColour", CallingConvention = CallingConvention.StdCall)]
        public static extern void SetKeyColor(float red, float green, float blue, float alpha);

        public static void SetKeyColor(Color color)
        {
            SetKeyColor(color.R, color.G, color.B, color.A);
        }

        public static void SetMemoryHint(MemoryHint hint)
        {
            IlHint(ILDefines.IL_MEM_SPEED_HINT, (uint)hint);
        }

        public static void SetCompressionHint(CompressionHint hint)
        {
            IlHint(ILDefines.IL_COMPRESSION_HINT, (uint)hint);
        }

        public static void SetJpgSaveFormat(JpgSaveFormat format)
        {
            IlSetInteger(ILDefines.IL_JPG_SAVE_FORMAT, (int)format);
        }

        public static void SetInteger(ILIntegerMode mode, int value)
        {
            IlSetInteger((uint)mode, value);
        }

        public static void SetOriginLocation(OriginLocation origin)
        {
            IlOriginFunc((uint)origin);
        }

        public static void SetString(ILStringMode mode, string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            IlSetString((uint)mode, value);
        }

        public static void SetQuantization(Quantization mode)
        {
            IlSetInteger(ILDefines.IL_QUANTIZATION_MODE, (int)mode);
        }

        public static bool SetPixels(int xOffset, int yOffset, int zOffset, int width, int height, int depth, DataFormat format, DataType dataType, byte[] data)
        {
            if (data == null || data.Length == 0)
                return false;

            if (xOffset < 0 || yOffset < 0 || zOffset < 0 || width < 1 || height < 1 || depth < 1)
                return false;

            uint size = (uint)data.Length;

            unsafe
            {
                fixed (byte* ptr = data)
                {
                    IlSetPixels(xOffset, yOffset, zOffset, (uint)width, (uint)height, (uint)depth, (uint)format, (uint)dataType, new IntPtr(ptr));
                }
            }
            return true;
        }

        /// <summary>
        /// Shuts DevIL's subsystem down, freeing up memory allocated for images. After this call is made, to use the wrapper again you
        /// need to call Initialize().
        /// </summary>
        public static void Shutdown()
        {
            if (_init)
            {
                IlShutDown();
                _init = false;
            }
        }

        /// <summary>
        /// Converts the currently bound surface (image, mipmap, etc) to the specified compressed format.
        /// </summary>
        /// <param name="format">Comrpessed format</param>
        /// <returns>True if the operation was successful or not.</returns>
        public static bool SurfaceToDxtcData(CompressedDataFormat format)
        {
            return IlSurfaceToDxtcData((uint)format);
        }

        /// <summary>
        /// Resets the currently bounded image with the new parameters. This destroys all existing data.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <param name="bytesPerComponent"></param>
        /// <param name="format"></param>
        /// <param name="dataType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool SetTexImage(int width, int height, int depth, DataFormat format, DataType dataType, byte[] data)
        {
            if (data == null || data.Length == 0)
                return false;

            byte bpp = (byte)MemoryHelper.GetFormatComponentCount(format);

            unsafe
            {
                fixed (byte* ptr = data)
                {
                    return IlTexImage((uint)width, (uint)height, (uint)depth, bpp, (uint)format, (uint)dataType, new IntPtr(ptr));
                }
            }

        }

        /// <summary>
        /// Resets the currently bounded image with the new parameters. This destroys all existing data.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <param name="format"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool SetTexImageDxtc(int width, int height, int depth, CompressedDataFormat format, byte[] data)
        {
            if (data == null || data.Length == 0)
                return false;
            unsafe
            {
                fixed (byte* ptr = data)
                {
                    return IlTexImageDxtc(width, height, depth, (uint)format, new IntPtr(ptr));
                }
            }
        }

        #endregion

        #region Library Info

        public static string GetVendorName()
        {
            IntPtr value = IlGetString(ILDefines.IL_VENDOR);
            if (value != IntPtr.Zero)
            {
                return Marshal.PtrToStringAnsi(value);
            }
            return "DevIL";
        }

        public static string GetVersion()
        {
            IntPtr value = IlGetString(ILDefines.IL_VERSION_NUM);
            if (value != IntPtr.Zero)
            {
                return Marshal.PtrToStringAnsi(value);
            }
            return "Unknown Version";
        }

        public static string[] GetImportExtensions()
        {
            IntPtr value = IlGetString(ILDefines.IL_LOAD_EXT);
            if (value != IntPtr.Zero)
            {
                string ext = Marshal.PtrToStringAnsi(value);
                string[] exts = ext.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < exts.Length; i++)
                {
                    string str = exts[i];
                    //Fix for what looks like a bug: Two entries don't have a space between them, whatmore the dds is
                    //a duplicate anyways
                    if (str.Equals("dcmdds"))
                    {
                        str = str.Substring(0, "dcm".Length);
                    }
                    exts[i] = "." + str;
                }
                return exts;
            }
            return new string[0];
        }

        public static string[] GetExportExtensions()
        {
            IntPtr value = IlGetString(ILDefines.IL_SAVE_EXT);
            if (value != IntPtr.Zero)
            {
                string ext = Marshal.PtrToStringAnsi(value);
                string[] exts = ext.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < exts.Length; i++)
                {
                    exts[i] = "." + exts[i];
                }

                return exts;
            }
            return new string[0];
        }
        #endregion

        #region IL Native Methods

        //Removed ilRegisterFormat to ilResetWrite. Might add the callbacks and reset mem stuff.
        //Also removed all load/saves/etc using file handles. Removed get int/bool versions using pass by ref
        //Removed SetMemory, SetRead, SetWrite, GetLumpPos

        [DllImport(ILDLL, EntryPoint = "ilActiveFace", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlActiveFace(uint Number);

        [DllImport(ILDLL, EntryPoint = "ilActiveImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlActiveImage(uint Number);

        [DllImport(ILDLL, EntryPoint = "ilActiveLayer", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlActiveLayer(uint Number);

        [DllImport(ILDLL, EntryPoint = "ilActiveMipmap", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlActiveMipmap(uint Number);

        ///InProfile: char*
        ///OutProfile: char*
        [DllImport(ILDLL, EntryPoint = "ilApplyProfile", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlApplyProfile(IntPtr InProfile, IntPtr OutProfile);

        [DllImport(ILDLL, EntryPoint = "ilBindImage", CallingConvention = CallingConvention.StdCall)]
        private static extern void IlBindImage(uint Image);

        [DllImport(ILDLL, EntryPoint = "ilBlit", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlBlit(uint Source, int DestX, int DestY, int DestZ, uint SrcX, uint SrcY, uint SrcZ, uint Width, uint Height, uint Depth);

        [DllImport(ILDLL, EntryPoint = "ilCloneCurImage", CallingConvention = CallingConvention.StdCall)]
        private static extern uint IlCloneCurImage();

        [DllImport(ILDLL, EntryPoint = "ilCompressDXT", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr IlCompressDXT(IntPtr Data, uint Width, uint Height, uint Depth, uint DXTCFormat, ref uint DXTCSize);

        [DllImport(ILDLL, EntryPoint = "ilCompressFunc", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlCompressFunc(uint Mode);

        [DllImport(ILDLL, EntryPoint = "ilConvertImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlConvertImage(uint DestFormat, uint DestType);

        [DllImport(ILDLL, EntryPoint = "ilConvertPal", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlConvertPal(uint DestFormat);

        [DllImport(ILDLL, EntryPoint = "ilCopyImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlCopyImage(uint Src);

        /// Return Type: sizeOfData
        ///Data: void*
        [DllImport(ILDLL, EntryPoint = "ilCopyPixels", CallingConvention = CallingConvention.StdCall)]
        private static extern uint IlCopyPixels(uint XOff, uint YOff, uint ZOff, uint Width, uint Height, uint Depth, uint Format, uint Type, IntPtr Data);

        /// Looks like creates a subimage @ the num index and type is IL_SUB_* (Next, Mip, Layer), etc
        [DllImport(ILDLL, EntryPoint = "ilCreateSubImage", CallingConvention = CallingConvention.StdCall)]
        private static extern uint IlCreateSubImage(uint Type, uint Num);

        [DllImport(ILDLL, EntryPoint = "ilDeleteImage", CallingConvention = CallingConvention.StdCall)]
        private static extern void IlDeleteImage(uint Num);

        /// Num is a Size_t
        [DllImport(ILDLL, EntryPoint = "ilDeleteImages", CallingConvention = CallingConvention.StdCall)]
        private static extern void IlDeleteImages(UIntPtr Num, uint[] Images);

        /// Return Type: Image Type
        ///FileName: char*
        [DllImport(ILDLL, EntryPoint = "ilDetermineType", CallingConvention = CallingConvention.StdCall)]
        private static extern uint IlDetermineType([In()][MarshalAs(UnmanagedType.LPStr)] string FileName);

        /// Return Type: Image Type
        ///Lump: void*
        [DllImport(ILDLL, EntryPoint = "ilDetermineTypeL", CallingConvention = CallingConvention.StdCall)]
        private static extern uint IlDetermineTypeL(IntPtr Lump, uint Size);

        [DllImport(ILDLL, EntryPoint = "ilDisable", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlDisable(uint Mode);

        [DllImport(ILDLL, EntryPoint = "ilEnable", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlEnable(uint Mode);

        [DllImport(ILDLL, EntryPoint = "ilFormatFunc", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlFormatFunc(uint Mode);

        ///Num: ILsizei->size_t->unsigned int
        [DllImport(ILDLL, EntryPoint = "ilGenImages", CallingConvention = CallingConvention.StdCall)]
        private static extern void IlGenImages(UIntPtr Num, uint[] Images);

        [DllImport(ILDLL, EntryPoint = "ilGenImage", CallingConvention = CallingConvention.StdCall)]
        private static extern uint IlGenImage();

        /// Return Type: ILubyte*
        ///Type: ILenum->unsigned int (Data type)
        [DllImport(ILDLL, EntryPoint = "ilGetAlpha", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr IlGetAlpha(uint Type);

        /// Return Type: ILubyte*
        [DllImport(ILDLL, EntryPoint = "ilGetData", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr IlGetData();

        /// Returns Size of Data, set Zero for BufferSize to get size initially.
        [DllImport(ILDLL, EntryPoint = "ilGetDXTCData", CallingConvention = CallingConvention.StdCall)]
        private static extern uint IlGetDXTCData(IntPtr Buffer, uint BufferSize, uint DXTCFormat);

        /// Return Type: Error type
        [DllImport(ILDLL, EntryPoint = "ilGetError", CallingConvention = CallingConvention.StdCall)]
        private static extern uint IlGetError();

        [DllImport(ILDLL, EntryPoint = "ilGetInteger", CallingConvention = CallingConvention.StdCall)]
        internal static extern int IlGetInteger(uint Mode);

        /// Return Type: ILubyte*, need to find size via current image's pal size
        [DllImport(ILDLL, EntryPoint = "ilGetPalette", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr IlGetPalette();

        /// Return Type: char*
        ///StringName: ILenum->unsigned int - String type enum
        [DllImport(ILDLL, EntryPoint = "ilGetString", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr IlGetString(uint StringName);

        ///Target: ILenum->unsigned int --> Type of hint
        ///Mode: ILenum->unsigned int ---> Hint value
        [DllImport(ILDLL, EntryPoint = "ilHint", CallingConvention = CallingConvention.StdCall)]
        private static extern void IlHint(uint Target, uint Mode);

        [DllImport(ILDLL, EntryPoint = "ilInit", CallingConvention = CallingConvention.StdCall)]
        private static extern void IlInit();

        /// Format Type
        [DllImport(ILDLL, EntryPoint = "ilImageToDxtcData", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlImageToDxtcData(uint Format);

        //Enable enum
        [DllImport(ILDLL, EntryPoint = "ilIsDisabled", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlIsDisabled(uint Mode);

        //Enable enum
        [DllImport(ILDLL, EntryPoint = "ilIsEnabled", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlIsEnabled(uint Mode);

        ///Checks if valid image - input is image id
        [DllImport(ILDLL, EntryPoint = "ilIsImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlIsImage(uint Image);

        ///Type: ILenum->unsigned int -- ImageType
        ///FileName: char*
        [DllImport(ILDLL, EntryPoint = "ilIsValid", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlIsValid(uint Type, [In()][MarshalAs(UnmanagedType.LPStr)] string FileName);

        /// Return Type: ILboolean->unsigned char - Image Type
        ///Lump: void*
        [DllImport(ILDLL, EntryPoint = "ilIsValidL", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlIsValidL(uint Type, IntPtr Lump, uint Size);

        /// Type is Image Type
        [DllImport(ILDLL, EntryPoint = "ilLoad", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlLoad(uint Type, [In()][MarshalAs(UnmanagedType.LPStr)] string FileName);

        /// Type is Image Type
        ///Lump: void*
        [DllImport(ILDLL, EntryPoint = "ilLoadL", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlLoadL(uint Type, IntPtr Lump, uint Size);

        /// Mode is origin type
        [DllImport(ILDLL, EntryPoint = "ilOriginFunc", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlOriginFunc(uint Mode);

        /// SRC image, and coords are the offsets in a blit
        [DllImport(ILDLL, EntryPoint = "ilOverlayImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlOverlayImage(uint Source, int XCoord, int YCoord, int ZCoord);

        /// Attribute bit flags
        [DllImport(ILDLL, EntryPoint = "ilPushAttrib", CallingConvention = CallingConvention.StdCall)]
        private static extern void IlPushAttrib(uint Bits);

        /// Image Type
        [DllImport(ILDLL, EntryPoint = "ilSave", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlSave(uint Type, [In()][MarshalAs(UnmanagedType.LPStr)] string FileName);

        [DllImport(ILDLL, EntryPoint = "ilSaveImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlSaveImage([In()][MarshalAs(UnmanagedType.LPStr)] string FileName);

        ///ImageType, similar deal with GetDXTCData - returns size, pass in a NULL for lump to determine size
        [DllImport(ILDLL, EntryPoint = "ilSaveL", CallingConvention = CallingConvention.StdCall)]
        private static extern uint IlSaveL(uint Type, IntPtr Lump, uint Size);

        ///Data: void*
        [DllImport(ILDLL, EntryPoint = "ilSetData", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlSetData(IntPtr Data);

        [DllImport(ILDLL, EntryPoint = "ilSetDuration", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlSetDuration(uint Duration);

        /// IntegerMode, and param is value
        [DllImport(ILDLL, EntryPoint = "ilSetInteger", CallingConvention = CallingConvention.StdCall)]
        private static extern void IlSetInteger(uint Mode, int Param);

        ///Data: void*, dataFormat and DataType
        [DllImport(ILDLL, EntryPoint = "ilSetPixels", CallingConvention = CallingConvention.StdCall)]
        private static extern void IlSetPixels(int XOff, int YOff, int ZOff, uint Width, uint Height, uint Depth, uint Format, uint Type, IntPtr Data);

        /// Return Type: void
        ///Mode: ILenum->unsigned int
        ///String: char*
        [DllImport(ILDLL, EntryPoint = "ilSetString", CallingConvention = CallingConvention.StdCall)]
        private static extern void IlSetString(uint Mode, [In()][MarshalAs(UnmanagedType.LPStr)] string String);

        [DllImport(ILDLL, EntryPoint = "ilShutDown", CallingConvention = CallingConvention.StdCall)]
        private static extern void IlShutDown();

        /// compressed DataFormat
        [DllImport(ILDLL, EntryPoint = "ilSurfaceToDxtcData", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlSurfaceToDxtcData(uint Format);

        /// dataFormat and DataType, destroys current data
        /// Bpp (NumChannels) bytes per pixel - e.g. 3 for RGB
        ///Data: void*
        [DllImport(ILDLL, EntryPoint = "ilTexImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlTexImage(uint Width, uint Height, uint Depth, byte Bpp, uint Format, uint Type, IntPtr Data);

        ///DxtcForamt is CompressedDataFormat, destroys current data
        [DllImport(ILDLL, EntryPoint = "ilTexImageDxtc", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlTexImageDxtc(int Width, int Height, int Depth, uint DxtFormat, IntPtr Data);

        ///Image type from extension of file
        [DllImport(ILDLL, EntryPoint = "ilTypeFromExt", CallingConvention = CallingConvention.StdCall)]
        private static extern uint IlTypeFromExt([In()][MarshalAs(UnmanagedType.LPStr)] string FileName);

        ///Sets the current DataType
        [DllImport(ILDLL, EntryPoint = "ilTypeFunc", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlTypeFunc(uint Mode);

        //Loads raw data from a file, bpp is only valid for 1, 3, 4
        [DllImport(ILDLL, EntryPoint = "ilLoadData", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlLoadData([In()][MarshalAs(UnmanagedType.LPStr)] string FileName, uint Width, uint Height, uint Depth, byte Bpp);

        //Loads raw data from a lump, bpp is only valid for 1, 3, 4
        [DllImport(ILDLL, EntryPoint = "ilLoadDataL", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IlLoadDataL(IntPtr Lump, uint Size, uint Width, uint Height, uint Depth, byte Bpp);

        #endregion
    }
}
