using System.Collections.Generic;
using ExcelDataReader;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ExcelReader
{
    public struct ExcelData{
        // 发言人的名字
        public string speakerName;
        // 发言人的内容
        public string speakerContent;
        // 发言人头像的文件名
        public string avatarImageFileName;
        // 发言人声音的文件名
        public string vocalAudioFileName;
        // 背景音乐的文件名
        public string backgroundMusicFileName;
        // 环境音效的文件名
        public string soundAudioFileName;
        // 背景图片的文件名
        public string backgroundFileName;
        // 背景蒙版的文件名
        public string maskFileName;
        // 物品图片的文件名
        public string itemFileName;
        // 人物 1行为
        public string character1Action;
        // 人物 1立绘
        public string character1ImageFileName;
        // 人物 2行为
        public string character2Action;
        // 人物 2立绘
        public string character2ImageFileName;        
        // 人物 3行为
        public string character3Action;
        // 人物 3立绘
        public string character3ImageFileName;
    }

    public static List<ExcelData> ReadExcel(string fileName) {
        List<ExcelData> excelData = new List<ExcelData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
        // 构建完整路径（StreamingAssets专用）
        string filePath = Path.Combine(Application.streamingAssetsPath, Constants.STORY_PATH, fileName + Constants.EXCEL_FILE_EXTENSION);
        Debug.Log(Application.streamingAssetsPath);
        Debug.Log(Constants.STORY_PATH);
        Debug.Log(fileName + Constants.EXCEL_FILE_EXTENSION);
        Debug.Log(filePath);
        // 使用 UnityWebRequest 异步读取（同步方式需在协程中调用）
        using (UnityWebRequest www = UnityWebRequest.Get(filePath)) {
            www.SendWebRequest();
            
            while (!www.isDone) {
                // 等待加载完成（如需同步，可在此处阻塞）
            }
            
            if (www.result == UnityWebRequest.Result.Success) {
                byte[] fileData = www.downloadHandler.data;
                using (MemoryStream stream = new MemoryStream(fileData)) {
                    using (var reader = ExcelReaderFactory.CreateReader(stream)) {
                        // 原有解析逻辑不变
                        do {
                            while (reader.Read()) {
                                ExcelData data = new ExcelData();
                                data.speakerName = reader.IsDBNull(0) ? string.Empty : reader.GetValue(0)?.ToString();
                                data.speakerContent = reader.IsDBNull(1) ? string.Empty : reader.GetValue(1)?.ToString();
                                data.avatarImageFileName = reader.IsDBNull(2) ? string.Empty : reader.GetValue(2)?.ToString();
                                data.backgroundFileName = reader.IsDBNull(3) ? string.Empty : reader.GetValue(3)?.ToString();
                                data.maskFileName = reader.IsDBNull(4) ? string.Empty : reader.GetValue(4)?.ToString();
                                data.itemFileName = reader.IsDBNull(5) ? string.Empty : reader.GetValue(5)?.ToString();
                                data.vocalAudioFileName = reader.IsDBNull(6) ? string.Empty : reader.GetValue(6)?.ToString();
                                data.soundAudioFileName = reader.IsDBNull(7) ? string.Empty : reader.GetValue(7)?.ToString();
                                data.backgroundMusicFileName = reader.IsDBNull(8) ? string.Empty : reader.GetValue(8)?.ToString();
                                data.character1Action = reader.IsDBNull(9) ? string.Empty : reader.GetValue(9)?.ToString();
                                data.character1ImageFileName = reader.IsDBNull(10) ? string.Empty : reader.GetValue(10)?.ToString();
                                data.character2Action = reader.IsDBNull(11) ? string.Empty : reader.GetValue(11)?.ToString();
                                data.character2ImageFileName = reader.IsDBNull(12) ? string.Empty : reader.GetValue(12)?.ToString();
                                data.character3Action = reader.IsDBNull(13) ? string.Empty : reader.GetValue(13)?.ToString();
                                data.character3ImageFileName = reader.IsDBNull(14) ? string.Empty : reader.GetValue(14)?.ToString();                        

                                excelData.Add(data);  
                            }
                        } while (reader.NextResult());
                    }
                }
            } 
            else
            {
                Debug.LogError("Excel文件读取失败：" + www.error);
            }
        }
        return excelData;
    }
}
