using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
   public class GetLogo
    {
        public string ShowEmployeePhoto(string UserID, string empPhoto)
        {
            return "Handlers/ShowEmployeeFiles.ashx?UserID=" + UserID + "&Photo=" + empPhoto;
        }
        public string ShowAadhar(string UserID, string Aadhar)
        {
            return "Handlers/ShowEmployeeFiles.ashx?UserID=" + UserID + "&Aadhar=" + Aadhar;
        }
        public string ShowPAN(string UserID, string PAN)
        {
            return "Handlers/ShowEmployeeFiles.ashx?UserID=" + UserID + "&PAN=" + PAN;
        }
        public string ShowServiceTypeIcon(string ServiceTypeID, string Icon)
        {
            return "Handlers/ShowServiceTypeIcons.ashx?ServiceTypeID=" + ServiceTypeID + "&Icon=" + Icon;
        }
        public string ShowLotImage(string LotID, string LotImage)
        {
            return "Handlers/ShowLotImage.ashx?LotID=" + LotID + "&Image=" + LotImage;
        }
        public string ShowLotImage2(string LotID, string LotImage2)
        {
            return "Handlers/ShowLotImage.ashx?LotID=" + LotID + "&Image2=" + LotImage2;
        }
        public string ShowLotImage3(string LotID, string LotImage3)
        {
            return "Handlers/ShowLotImage.ashx?LotID=" + LotID + "&Image3=" + LotImage3;
        }
    }
}
