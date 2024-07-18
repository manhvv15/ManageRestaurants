using NPOI.SS.UserModel;

namespace ManageRestaurant.Services
{
    public class Excel
    {
        public string CheckFileMau(ISheet sheet, string headerTitle, int startRow)
        {
            try
            {
                IRow row = sheet.GetRow(0);
                if (row != null)
                {
                    var txtHeadear = row.GetCell(0)?.ToString();
                    if (txtHeadear != headerTitle)
                    {
                        return "Vui lòng import file đúng định dạng";
                    }
                    else
                    {
                        if (sheet.PhysicalNumberOfRows < startRow)
                        {
                            return "File import không có dữ liệu";
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                }
                return "File import không có dữ liệu";
            }
            catch (Exception)
            {
                return "Vui lòng import file đúng định dạng";
            }
        }

    }
}
