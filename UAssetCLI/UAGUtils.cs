﻿using System;
using System.Collections.Generic;
 using System.ComponentModel;
 using System.Diagnostics;
// using System.Drawing;
using System.Reflection;
using System.Text;
// using System.Windows.Forms;
using UAssetAPI;
using UAssetAPI.UnrealTypes;

namespace UAssetCLI
{
    public static class UAGUtils
    {
        internal static string _displayVersion = string.Empty;

        public static T TryGetElement<T>(this T[] array, int index)
        {
            if (array != null && index < array.Length)
            {
                return array[index];
            }
            return default(T);
        }

        public static object ArbitraryTryParse(this string input, Type type)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(type);
                if (converter != null)
                {
                    return converter.ConvertFromString(input);
                }
            }
            catch (NotSupportedException) { }
            return null;
        }

        // public static ContextMenuStrip MergeContextMenus(ContextMenuStrip one, ContextMenuStrip two)
        // {
        //     if (one == null) return two;
        //     if (two == null) return one;
        //
        //     one.Items.AddRange(two.Items);
        //     return one;
        // }
        //
        // // ((Form1)x.DataGridView.Parent).nameMapContext;
        // public static void UpdateContextMenuStripOfRow(DataGridViewRow x, ContextMenuStrip strip)
        // {
        //     x.ContextMenuStrip = MergeContextMenus(x.ContextMenuStrip, strip);
        //     x.HeaderCell.ContextMenuStrip = MergeContextMenus(x.HeaderCell.ContextMenuStrip, strip);
        //     foreach (DataGridViewCell y in x.Cells)
        //     {
        //         y.ContextMenuStrip = MergeContextMenus(y.ContextMenuStrip, strip);
        //     }
        // }
        //
        // public static void AdjustFormPosition(this Form frm1, Form overrideOwner = null)
        // {
        //     if (overrideOwner == null) overrideOwner = frm1.Owner;
        //     if (overrideOwner != null) frm1.Location = new Point((overrideOwner.Location.X + overrideOwner.Width / 2) - (frm1.Width / 2), (overrideOwner.Location.Y + overrideOwner.Height / 2) - (frm1.Height / 2));
        // }
        //
        // public static void UpdateObjectPropertyValues(UAsset asset, DataGridViewRow row, DataGridView dgv, FPackageIndex objData, int column = 3)
        // {
        //     if (dgv == null || row == null || objData == null) return;
        //
        //     bool underlineStyle = false;
        //     if (objData.IsImport() && objData == null)
        //     {
        //         row.Cells[column].Value = string.Empty;
        //     }
        //     else
        //     {
        //         row.Cells[column].Value = objData.IsExport() ? "Jump" : (objData.IsImport() ? objData.ToImport(asset)?.ObjectName?.ToString() : string.Empty);
        //         row.Cells[column].Tag = "CategoryJump";
        //         if (objData.IsExport()) underlineStyle = true;
        //     }
        //     row.Cells[column].ReadOnly = objData.IsImport();
        //
        //     DataGridViewCellStyle sty = new DataGridViewCellStyle();
        //     if (underlineStyle)
        //     {
        //         Font styFont = new Font(dgv.Font.Name, UAGPalette.RecommendedFontSize, FontStyle.Underline);
        //         sty.Font = styFont;
        //         sty.ForeColor = UAGPalette.LinkColor;
        //     }
        //     row.Cells[column].Style = sty;
        // }

        public static T[] StripNullsFromArray<T>(this T[] usArr)
        {
            int c = 0;
            for (int num = 0; num < usArr.Length; num++)
            {
                if (usArr[num] != null) c++;
            }

            var newData = new T[c];
            int indexAdded = 0;
            for (int num = 0; num < usArr.Length; num++)
            {
                if (usArr[num] != null) newData[indexAdded++] = usArr[num];
            }
            return newData;
        }

        public static List<T> StripNullsFromList<T>(this List<T> usList)
        {
            for (int num = 0; num < usList.Count; num++)
            {
                if (usList[num] == null)
                {
                    usList.RemoveAt(num);
                    num--;
                }
            }
            return usList;
        }

        public static string ConvertByteArrayToString(this byte[] val)
        {
            if (val == null) return "";
            return BitConverter.ToString(val).Replace("-", " ");
        }

        public static byte[] ConvertStringToByteArray(this string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return Array.Empty<byte>();
            string[] rawStringArr = val.Split(' ');
            byte[] byteArr = new byte[rawStringArr.Length];
            for (int i = 0; i < rawStringArr.Length; i++) byteArr[i] = Convert.ToByte(rawStringArr[i], 16);
            return byteArr;
        }

        // public static string ShortcutToText(Keys shortcutKeys)
        // {
        //     return TypeDescriptor.GetConverter(typeof(Keys)).ConvertToString(shortcutKeys);
        // }

        /*
            UAssetCLI versions are formatted as follows: MAJOR.MINOR.BUILD
            * MAJOR - incremented for very big changes or backwards-incompatible changes
            * MINOR - incremented for notable changes
            * BUILD - incremented for bug fixes or very small improvements
        */
        public static bool IsUAGVersionLower(this Version v1)
        {
            Version fullUagVersion = new Version(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
            return v1.CompareTo(fullUagVersion) > 0;
        }

        public static string FEncode(this FString val)
        {
            if (val == null) return FString.NullCase;
            return val.Value.Replace("\n", "\\n").Replace("\r", "\\r");
        }

        public static FString FDecode(this string val, string encodingHeaderName)
        {
            if (val == FString.NullCase) return null;
            return FString.FromString(val.Replace("\\n", "\n").Replace("\\r", "\r"), encodingHeaderName.Equals(Encoding.Unicode.HeaderName) ? Encoding.Unicode : Encoding.ASCII);
        }

        public static void OpenURL(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        public static void OpenDirectory(string dir)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = dir,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        // private static Control internalForm;
        // public static void InitializeInvoke(Control control)
        // {
        //     internalForm = control;
        // }

        // public static bool InvokeUI(Action act)
        // {
        //     if (internalForm == null) return false;
        //     if (internalForm.InvokeRequired)
        //     {
        //         internalForm.Invoke(act);
        //     }
        //     else
        //     {
        //         act();
        //     }
        //     return true;
        // }
    }
}
