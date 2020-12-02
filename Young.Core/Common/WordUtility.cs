using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace Young.Core.Common
{
    public class WordUtility
    {

        private object tempFile = null;
        private object saveFile = null;
        private static Word._Document wDoc = null;  //word文档
        private static Word._Application wApp = null; //word进程
        private object missing = System.Reflection.Missing.Value;
        private static Young.Core.Logger.ILog Logger = Young.Core.Logger.LoggerManager.Current();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tempFile">模板文件名称 如 报告.doc</param>
        /// <param name="saveFile">保存的文件名</param>
        public WordUtility(string tempFile, string saveFile)
        {
            this.tempFile = Path.Combine(@tempFile);
            this.saveFile = Path.Combine(@saveFile);
        }

        // $名子试试$
        // simpleExpPairValue.Add("$名子试试$","23232");
        /// <summary>
        /// 根据定义模板 生成 word
        /// </summary>
        /// <param name="simpleExpPairValue">简单的非重复型数据</param>
        /// <param name="dt">模版包含头部信息和表格，表格重复使用  重复表格的数据</param>
        /// <param name="expPairColumn">模版包含头部信息和表格，表格重复使用  word中要替换的表达式和表格字段的对应关系</param>
        public bool GenerateWord(Dictionary<string, string> simpleExpPairValue, DataTable dt = null, Dictionary<string, string> expPairColumn = null)
        {
            if (!File.Exists(tempFile.ToString()))
            {
                MessageUtil.ShowError(string.Format("{0}模版文件不存在，请先设置模版文件。", tempFile.ToString()));
                return false;
            }
            try
            {
                wApp = new Word.Application();

                wApp.Visible = false;

                wDoc = wApp.Documents.Add(ref tempFile, ref missing, ref missing, ref missing);

                wDoc.Activate();// 当前文档置前

                bool isGenerate = false;

                if (simpleExpPairValue != null && simpleExpPairValue.Count > 0)
                    isGenerate = ReplaceAllRang(simpleExpPairValue);

                // 表格有重复
                if (dt != null && dt.Rows.Count > 0 && expPairColumn != null && expPairColumn.Count > 0)
                    isGenerate = GenerateTable(dt, expPairColumn);

                if (isGenerate)
                    wDoc.SaveAs(ref saveFile, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);

                DisposeWord();

                return true;
            }
            catch
            {
                MessageUtil.ShowError("生成失败");
                return false;
            }
        }

        /// <summary>
        /// 根据定义的书签 生成 word
        /// </summary>
        /// <param name="dc">简单的非重复型数据</param>
        public bool GenerateWordByBookmarks(Dictionary<string, string> dc)
        {
            killWinWordProcess();

            if (!File.Exists(tempFile.ToString()))
            {
                MessageUtil.ShowError(string.Format("{0}模版文件不存在，请先设置模版文件。", tempFile.ToString()));
                return false;
            }
            try
            {
                wApp = new Word.Application();
                wApp.Visible = false;

                wDoc = wApp.Documents.Add(ref tempFile, ref missing, ref missing, ref missing);

                wDoc.Activate();// 当前文档置前

                // 替换书签
                foreach (Word.Bookmark bm in wDoc.Bookmarks)
                {
                    var bmName = bm.Name;
                    foreach (string item in dc.Keys)
                    {
                        if (bmName == item)
                        {
                            bm.Select();
                            bm.Range.Text = dc[item];
                        }
                    }
                }

                //Log.Info("ss", s);

                wDoc.SaveAs(ref saveFile, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);

                DisposeWord();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MessageUtil.ShowError("生成失败");
                return false;
            }
        }

        /// <summary>
        /// 生成 table 表格
        /// </summary>
        /// <param name="dt">重复表格数据</param>
        /// <param name="expPairColumn">对应关系</param>
        /// <returns></returns>
        private bool GenerateTable(DataTable dt, Dictionary<string, string> expPairColumn)
        {
            try
            {
                int tableNums = dt.Rows.Count;

                Word.Table tb = wDoc.Tables[1];

                tb.Range.Copy();

                Dictionary<string, object> dc = new Dictionary<string, object>();
                for (int i = 0; i < tableNums; i++)
                {
                    dc.Clear();

                    if (i == 0)
                    {
                        foreach (string key in expPairColumn.Keys)
                        {
                            string column = expPairColumn[key];
                            object value = null;
                            value = dt.Rows[i][column];
                            dc.Add(key, value);
                        }

                        ReplaceTableRang(wDoc.Tables[1], dc);
                        continue;
                    }

                    wDoc.Paragraphs.Last.Range.Paste();

                    foreach (string key in expPairColumn.Keys)
                    {
                        string column = expPairColumn[key];
                        object value = null;
                        value = dt.Rows[i][column];
                        dc.Add(key, value);
                    }

                    ReplaceTableRang(wDoc.Tables[1], dc);
                }


                return true;
            }
            catch (Exception ex)
            {
                DisposeWord();
                MessageBox.Show("生成模版里的表格失败。" + ex.Message);
                return false;
            }
        }

        private bool ReplaceTableRang(Word.Table table, Dictionary<string, object> dc)
        {
            try
            {
                object replaceArea = Word.WdReplace.wdReplaceAll;

                foreach (string item in dc.Keys)
                {
                    object replaceKey = item;
                    object replaceValue = dc[item];
                    table.Range.Find.Execute(ref replaceKey, ref missing, ref missing, ref missing,
                      ref missing, ref missing, ref missing, ref missing, ref missing,
                      ref replaceValue, ref replaceArea, ref missing, ref missing, ref missing,
                      ref missing);
                }
                return true;
            }
            catch (Exception ex)
            {
                DisposeWord();
                MessageUtil.ShowError(string.Format("{0}模版中没有找到指定的要替换的表达式。{1}", tempFile, ex.Message));
                return false;
            }
        }

        private bool ReplaceAllRang(Dictionary<string, string> dc)
        {
            try
            {
                object replaceArea = Word.WdReplace.wdReplaceAll;

                foreach (string item in dc.Keys)
                {
                    object replaceKey = item;
                    object replaceValue = dc[item];
                    wApp.Selection.Find.Execute(ref replaceKey, ref missing, ref missing, ref missing,
                      ref missing, ref missing, ref missing, ref missing, ref missing,
                      ref replaceValue, ref replaceArea, ref missing, ref missing, ref missing,
                      ref missing);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(string.Format("{0}模版中没有找到指定的要替换的表达式。{1}", tempFile, ex.Message));
                return false;
            }
        }

        private void DisposeWord()
        {
            object saveOption = Word.WdSaveOptions.wdSaveChanges;

            wDoc.Close(ref saveOption, ref missing, ref missing);

            saveOption = Word.WdSaveOptions.wdDoNotSaveChanges;

            wApp.Quit(ref saveOption, ref missing, ref missing); //关闭Word进程
        }

        /// <summary>
        /// 结束标题为空的word进程
        /// </summary>
        public void killWinWordProcess()
        {
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("WINWORD");
            foreach (System.Diagnostics.Process p in process)
            {
                if (p.MainWindowTitle == "")
                {
                    p.Kill();
                }
            }
        }
    }
}