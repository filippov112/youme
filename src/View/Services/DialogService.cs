using MessageBox = System.Windows.Forms.MessageBox;

namespace View.Services
{
    internal class DialogService : IDialogService
    {
        public string? ShowInputTextDialog(string description, string title, string defaultName)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(description, title, defaultName);
        }

        /// <summary>
        /// Прервать, повторить или проигнорировать в дальнейшем?
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public DialogResult ShowAbortRetryIgnoreDialog(string message)
        {
            var result = MessageBox.Show(message, "Внимание!", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
            if (result == System.Windows.Forms.DialogResult.Retry)
                return DialogResult.Retry;
            if (result == System.Windows.Forms.DialogResult.Ignore)
                return DialogResult.Ignore;
            return DialogResult.Abort;
        }
        /// <summary>
        /// Ошибка
        /// </summary>
        /// <param name="message"></param>
        public void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// Предупреждение
        /// </summary>
        /// <param name="message"></param>
        public void ShowWarning(string message)
        {
            MessageBox.Show(message, "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /// <summary>
        /// Да/нет?
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool ShowYesNoDialog(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes;
        }

        public string? ShowOpenFolderDialog()
        {
            var folderDialog = new FolderBrowserDialog();
            try
            {
                folderDialog.Description = "Выберите проект";
                folderDialog.UseDescriptionForTitle = true;

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return folderDialog.SelectedPath;
                }
            }
            finally
            {
                folderDialog.Dispose();
            }
            return null;
        }
    }

    /// <summary>
    /// Результат диалога с пользователем
    /// </summary>
    public enum DialogResult
    {
        Abort,
        Retry,
        Ignore
    }

    /// <summary>
    /// Сервис взаимодействия с пользователем по средством диалоговых окон
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Уведомить об ошибке
        /// </summary>
        /// <param name="message"></param>
        public void ShowError(string message);
        /// <summary>
        /// Показать предупреждение
        /// </summary>
        /// <param name="message"></param>
        public void ShowWarning(string message);
        /// <summary>
        /// Спросить что делать - прервать, повторить или проигнорировать в дальнейшем?
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public DialogResult ShowAbortRetryIgnoreDialog(string message);
        /// <summary>
        /// Задать да/нет-вопрос
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool ShowYesNoDialog(string message, string title);

        public string? ShowInputTextDialog(string description, string title, string defaultName);

        public string? ShowOpenFolderDialog();
    }
}
