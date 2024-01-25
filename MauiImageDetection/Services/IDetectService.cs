using MauiImageDetection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Common;

namespace MauiImageDetection.Services
{
    public interface IDetectService
    {
        DetectResult Detect(byte[] file);
    }
}
