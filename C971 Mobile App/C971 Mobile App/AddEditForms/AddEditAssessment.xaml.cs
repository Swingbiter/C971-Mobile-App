using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_Mobile_App.AddEditForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditAssessment : ContentPage
    {
        private bool edit_mode = false;
        private int courseId;
        private Assessment _assessment;
        public AddEditAssessment(Course course)
        {
            InitializeComponent();
            courseId = course.Id;
            lbl_add_edit.Text = "Add New Assessment";
            btn_add_save.Text = "Add Assessment";
            btn_del_assess.IsVisible = false;
        }

        public AddEditAssessment(Assessment assessment)
        {
            InitializeComponent();
            _assessment = assessment;
            lbl_add_edit.Text = "Edit";
            btn_add_save.Text = "Save";
            edit_mode = true;
            entry_name.Text = _assessment.Title;
            entry_type.Text = _assessment.Type;
            datepicker_start.Date = _assessment.StartDate;
            datepicker_end.Date = _assessment.EndDate;
            switch_notification.IsToggled = _assessment.Notification;
        }

        private void btn_add_save_Clicked(object sender, EventArgs e)
        {
            if ((datepicker_start.Date > datepicker_end.Date) || (datepicker_start.Date == datepicker_end.Date))
            {
                DisplayAlert("Date Error", "Starting Date can't be after or equal to end date", "OK");
                return;
            }

            if (string.IsNullOrEmpty(entry_type.Text) || string.IsNullOrEmpty(entry_name.Text))
            {
                DisplayAlert("Invalid Input", "Field(s) must not be empty", "OK");
                return;
            }
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            { 
                if (edit_mode)
                {
                    _assessment.Title = entry_name.Text;
                    _assessment.Type = entry_type.Text;
                    _assessment.StartDate = datepicker_start.Date;
                    _assessment.EndDate = datepicker_end.Date;
                    _assessment.Notification = switch_notification.IsToggled;

                    conn.Update(_assessment);

                    Navigation.PopAsync();
                }
                else
                {
                    Assessment new_assessment = new Assessment();
                    new_assessment.Course = courseId;
                    new_assessment.Title = entry_name.Text;
                    new_assessment.Type = entry_type.Text;
                    new_assessment.StartDate = datepicker_start.Date;
                    new_assessment.EndDate = datepicker_end.Date;
                    new_assessment.Notification = switch_notification.IsToggled;

                    conn.Insert(new_assessment);

                    Navigation.PopAsync();
                }
            }
        }

        private void btn_del_assess_Clicked(object sender, EventArgs e)
        {
            if (edit_mode)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    conn.Delete(_assessment);
                    Navigation.PopAsync();
                }
            }
            else
            {
                // shouldn't be able to click anways, just in case
                return;
            }
        }
    }
}