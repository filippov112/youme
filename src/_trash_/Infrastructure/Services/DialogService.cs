using Application.Services;
using Application.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Infrastructure.Services
{
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Прервать, повторить или проигнорировать в дальнейшем?
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Application.Types.DialogResult ShowAbortRetryIgnoreDialog(string message)
        {
            var result = MessageBox.Show(message, "Внимание!", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
            if (result == System.Windows.Forms.DialogResult.Retry)
                return Application.Types.DialogResult.Retry;
            if (result == System.Windows.Forms.DialogResult.Ignore)
                return Application.Types.DialogResult.Ignore;
            return Application.Types.DialogResult.Abort;
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
    }
}
