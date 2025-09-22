using System.Text;

namespace Youme.Services;

public static class StorageService
{
    // - ###input### - ������� �����
    // - ###project### - ��������� �������
    // - ###settings### - ���������������� ���������                                  
    // - ###request### - ������ ������������
    // - ###path### - ������������� ���� � �����
    // - ###content### - ���������� �����

    public const string KEY_WORD_INPUT = "###input###";
    public const string KEY_WORD_PROJECT = "###project###";
    public const string KEY_WORD_SETTINGS = "###settings###";
    public const string KEY_WORD_REQUEST = "###request###";
    public const string KEY_WORD_PATH = "###path###";
    public const string KEY_WORD_CONTENT = "###content###";

    public static string InputProjectPrompt { get; set; } = "������ ��� ���������� �� ��������� ��������. ��� ��� ���������:";
    public static string StructurePromptLocal { get; set; } = string.Empty;
    public static string StructurePromptGlobal { get; set; } = $"{KEY_WORD_INPUT}\n`````\n{KEY_WORD_PROJECT}\n`````\n{KEY_WORD_REQUEST}\n\n{KEY_WORD_SETTINGS}";
    public static string UserSettingsPrompt { get; set; } = "��� ������ ���� ������ � ���������.";
    public static string StyleFileBlock { get; set; } = $"����: {KEY_WORD_PATH}\n````\n{KEY_WORD_CONTENT}\n````\n";


    // ������ ��������� ������

    /// <summary>
    /// ��������� ���������� ������� � ������ ��������� �������, ���������������� �������� � ������� ������������
    /// </summary>
    /// <param name="projectStructure">���������� �������</param>
    /// <param name="userRequest">���������������� ������</param>
    /// <returns></returns>
    public static string GetPrompt(string projectStructure, string userRequest)
    {
        // ���� �� ����� ��������� ������ ���������, ���������� ����������
        string result = string.IsNullOrWhiteSpace(StructurePromptLocal) ? StructurePromptGlobal : StructurePromptLocal;
        
        result = result.Replace(KEY_WORD_INPUT, InputProjectPrompt);
        result = result.Replace(KEY_WORD_PROJECT, projectStructure);
        result = result.Replace(KEY_WORD_SETTINGS, UserSettingsPrompt);
        result = result.Replace(KEY_WORD_REQUEST, userRequest);
        return result;
    }

    /// <summary>
    /// ���������� ����� ����� ��� �������� � ������
    /// </summary>
    /// <param name="relativePath">���� � �����</param>
    /// <param name="fileContent">����������</param>
    /// <returns></returns>
    public static void AddFile(StringBuilder sb, string relativePath, string fileContent)
    {
        sb.AppendLine(
            StyleFileBlock
            .Replace(KEY_WORD_PATH, relativePath)
            .Replace(KEY_WORD_CONTENT, fileContent)
        );
    }

    public static string ProjectFolder { get; set; } = string.Empty;
}