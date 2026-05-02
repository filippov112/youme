using Presentation.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using DialogResult = Presentation.Enums.DialogResult;

namespace Presentation.Interfaces
{
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
    }
}
