using System;
using PaperPlaneTools;

namespace GreenNacho.AppManagement
{
    public struct AlertButton
    {
        public string text;
        public Action action;

        public AlertButton(string t, Action a = null)
        {
            text = t;
            action = a;
        }
    }

    public class AppAlertDispatcher
    {
        AlertUnityUIAdapter customAlertAdapter;

        public AppAlertDispatcher(AlertUnityUIAdapter customAlertAdapter)
        {
            this.customAlertAdapter = customAlertAdapter;
        }

        void SetUpAlertButtons(ref Alert alert, AlertButton positiveButton, AlertButton negativeButton = default, 
                                AlertButton neutralButton = default)
        {
            alert.SetPositiveButton(positiveButton.text, positiveButton.action);
            if (negativeButton.text != null)
                alert.SetNegativeButton(negativeButton.text, negativeButton.action);
            if (neutralButton.text != null)
                alert.SetNeutralButton(neutralButton.text, neutralButton.action);
        }

        public void ShowNativeWarningAlert(string title, string message, string positiveText, Action positiveAction = null)
        {
            Alert alert = new Alert(title, message); 
            AlertButton positiveButton = new AlertButton(positiveText, positiveAction);

            SetUpAlertButtons(ref alert, positiveButton);
            alert.Show();
        }

        public void ShowCustomWarningAlert(string title, string message, string positiveText, Action positiveAction = null)
        {
            Alert alert = new Alert(title, message);
            AlertButton positiveButton = new AlertButton(positiveText, positiveAction);
            
            SetUpAlertButtons(ref alert, positiveButton);
            alert.SetAdapter(customAlertAdapter);
            alert.Show();
        }

        public void ShowNativeConfirmationAlert(string title, string message, string positiveText, string negativeText, 
                                                Action positiveAction = null, Action negativeAction = null) 
        {
            Alert alert = new Alert(title, message);
            AlertButton positiveButton = new AlertButton(positiveText, positiveAction);          
            AlertButton negativeButton = new AlertButton(negativeText, negativeAction);          
            
            SetUpAlertButtons(ref alert, positiveButton, negativeButton);
            alert.Show();
        }

        public void ShowCustomConfirmationAlert(string title, string message, string positiveText, string negativeText,
                                                Action positiveAction = null, Action negativeAction = null) 
        {
            Alert alert = new Alert(title, message);
            AlertButton positiveButton = new AlertButton(positiveText, positiveAction);
            AlertButton negativeButton = new AlertButton(negativeText, negativeAction);
            
            SetUpAlertButtons(ref alert, positiveButton, negativeButton);
            alert.SetAdapter(customAlertAdapter);            
            alert.Show();
        }
    }
}