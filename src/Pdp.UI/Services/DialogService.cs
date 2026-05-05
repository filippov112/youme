using Pdp.UI.Interfaces;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Pdp.UI.Services
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
        public Enums.DialogResult ShowAbortRetryIgnoreDialog(string message)
        {
            var result = MessageBox.Show(message, "Внимание!", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
            if (result == System.Windows.Forms.DialogResult.Retry)
                return Enums.DialogResult.Retry;
            if (result == System.Windows.Forms.DialogResult.Ignore)
                return Enums.DialogResult.Ignore;
            return Enums.DialogResult.Abort;
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
}
