using System.Runtime.InteropServices;
using System.Text;
  
/// <summary>
/// INIファイルを読み書きするクラス
/// </summary>
public class INIFile
{
    private string path;    // ファイルパス
  
    /// <summary>
    /// ファイル名を指定して初期化します。
    /// ファイルが存在しない場合は初回書き込み時に作成されます。
    /// </summary>
    public INIFile(string path)
    {
        this.path = path;
    }
  
    /// <summary>
    /// sectionとkeyからiniファイルの設定値を取得、設定します。 
    /// </summary>
    /// <returns>指定したsectionとkeyの組合せが無い場合は""が返ります。</returns>
    public string this[string section, string key]
    {
        set
        {
            WritePrivateProfileString(section, key, value, path);
        }
        get
        {
            StringBuilder sb = new StringBuilder(256);
            GetPrivateProfileString(section, key, string.Empty, sb, sb.Capacity, path);
            return sb.ToString();
        }
    }
  
    /// <summary>
    /// sectionとkeyからiniファイルの設定値を取得します。
    /// 指定したsectionとkeyの組合せが無い場合はdefaultvalueで指定した値が返ります。
    /// </summary>
    /// <returns>
    /// 指定したsectionとkeyの組合せが無い場合はdefaultvalueで指定した値が返ります。
    /// </returns>
    public string GetValue(string section, string key, string defaultvalue)
    {
        StringBuilder sb = new StringBuilder(256);
        GetPrivateProfileString(section, key, defaultvalue, sb, sb.Capacity, path);
        return sb.ToString();
    }
     
    /// <summary>
    /// 指定された .ini ファイルの指定されたセクション内にある、指定されたキーに関連付けられている文字列を取得します
    /// </summary>
    /// <param name="lpAppName">セクション名</param>
    /// <param name="lpKeyName">キー名</param>
    /// <param name="lpDefault">既定の文字列</param>
    /// <param name="lpReturnedString">情報が格納されるバッファ</param>
    /// <param name="nSize">情報バッファのサイズ</param>
    /// <param name="lpFileName">.ini ファイルの名前</param>
    /// <returns>関数が成功するとバッファに格納された文字数が返ります</returns>
    [DllImport("kernel32.dll")]
    private static extern int GetPrivateProfileString(
        string lpAppName, 
        string lpKeyName, 
        string lpDefault, 
        StringBuilder lpReturnedString, 
        int nSize, 
        string lpFileName);
  
    /// <summary>
    /// 指定された .ini ファイルの、指定されたセクション内に、指定されたキー名とそれに関連付けられた文字列を格納します
    /// </summary>
    /// <param name="lpAppName">セクション名</param>
    /// <param name="lpKeyName">キー名</param>
    /// <param name="lpString">追加するべき文字列</param>
    /// <param name="lpFileName">.ini ファイルの名前</param>
    /// <returns>関数が文字列を .ini ファイルに格納することに成功すると 0 以外の値が返ります</returns>
    [DllImport("kernel32.dll")]
    private static extern int WritePrivateProfileString(
        string lpAppName,
        string lpKeyName, 
        string lpString, 
        string lpFileName);
}