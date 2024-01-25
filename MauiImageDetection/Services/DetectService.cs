﻿using MauiImageDetection.Models;
using NcnnDotNet;
using NcnnDotNet.OpenCV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraFaceDotNet;
using Mat = NcnnDotNet.Mat;

namespace MauiImageDetection.Services
{
    public sealed class DetectService : IDetectService
    {

        #region Fields

        private readonly UltraFace _UltraFace;

        #endregion

        #region Constructors

        //public DetectService()
        //{
        //    var resourcePrefix = "MauiImageDetection.data.";
        //    // note that the prefix includes the trailing period '.' that is required
        //    var files = new[] { "RFB-320.bin", "RFB-320.param" };
        //    var assembly = System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(DetectService)).Assembly;
        //    foreach (var file in files)
        //    {
        //        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), file);
        //        using (var fs = File.Create(path))
        //        using (var stream = assembly.GetManifestResourceStream(path))
        //        {
        //            stream.Seek(0, SeekOrigin.Begin);
        //            stream.CopyTo(fs);
        //        }
        //    }

        //    var binPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), files[0]);
        //    var paramPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), files[1]);

        //    var param = new UltraFaceParameter
        //    {
        //        BinFilePath = binPath,
        //        ParamFilePath = paramPath,
        //        InputWidth = 320,
        //        InputLength = 240,
        //        NumThread = 1,
        //        ScoreThreshold = 0.7f
        //    };

        //    this._UltraFace = UltraFace.Create(param);
        //}

        public DetectService()
        {
            var resourcePrefix = "MauiImageDetection.data.";
            // note that the prefix includes the trailing period '.' that is required
            var files = new[] { "RFB-320.bin", "RFB-320.param" };
            var assembly = System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(DetectService)).Assembly;
            foreach (var file in files)
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), file);
                using (var fs = File.Create(path))
                {
                    // Corrected code: Use the correct resource name by combining the prefix and file name
                    var resourceFullName = path;// resourcePrefix + file;
                    using (var stream = assembly.GetManifestResourceStream(resourceFullName))
                    {
                        if (stream == null)
                        {
                            // Handle missing resource stream appropriately
                            //throw new Exception($"Resource not found: {resourceFullName}");
                        }
                        else
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            stream.CopyTo(fs);
                        }
                    }
                }
            }

            var binPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), files[0]);
            var paramPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), files[1]);

            var param = new UltraFaceParameter
            {
                BinFilePath = binPath,
                ParamFilePath = paramPath,
                InputWidth = 320,
                InputLength = 240,
                NumThread = 1,
                ScoreThreshold = 0.7f
            };

            this._UltraFace = UltraFace.Create(param);
        }


        #endregion

        #region IDetectService Members

        public DetectResult Detect(byte[] file)
        {
            using var frame = Cv2.ImDecode(file, CvLoadImage.Grayscale);
            if (frame.IsEmpty)
                throw new NotSupportedException("This file is not supported!!");

            if (Ncnn.IsSupportVulkan)
                Ncnn.CreateGpuInstance();

            using var inMat = Mat.FromPixels(frame.Data, NcnnDotNet.PixelType.Bgr2Rgb, frame.Cols, frame.Rows);

            var faceInfos = this._UltraFace.Detect(inMat).ToArray();

            if (Ncnn.IsSupportVulkan)
                Ncnn.DestroyGpuInstance();

            return new DetectResult(frame.Cols, frame.Rows, faceInfos);
        }

        #endregion
    }
}
