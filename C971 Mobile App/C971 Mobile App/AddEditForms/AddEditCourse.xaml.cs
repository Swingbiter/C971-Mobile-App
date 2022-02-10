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
    public partial class AddEditCourse : ContentPage
    {
        private bool edit_mode = false;
        private Course _course;
        private List<String> pickerList = new List<string>{"In-Progress", "Completed", "Plan-To-Take", "Dropped", "Not-Taken"};
        private int termId;
        public AddEditCourse(Term term)
        {
            termId = term.Id;
            InitializeComponent();
            lbl_add_edit.Text = "Add New Course";
            btn_add_save.Text = "Add";
            picker_status.ItemsSource = pickerList;
            picker_status.SelectedIndex = 4; // set to not-taken
        }

        public AddEditCourse(Course course)
        {
            InitializeComponent();
            _course = course;
            edit_mode = true;
            lbl_add_edit.Text = "Edit";
            btn_add_save.Text = "Save";
            entry_course_name.Text = _course.CourseName;
            datepicker_start.Date = _course.StartDate;
            datepicker_end.Date = _course.EndDate;
            entry_instructor_name.Text = _course.Instructor;
            entry_instructor_phone.Text = _course.InstructorPhone;
            entry_instructor_email.Text = _course.InstructorEmail;
            switch_notification.IsToggled = _course.Notification;
            entry_notes.Text = _course.Notes;
            picker_status.ItemsSource = pickerList;

            if (pickerList.Contains(_course.Status))
            {
                picker_status.SelectedIndex = pickerList.IndexOf(_course.Status);
            }
            else
            {
                picker_status.SelectedIndex = 4; // set to not-taken if status is somehow wrong
            }    
        }

        private void btn_add_save_Clicked(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                if ((datepicker_start.Date > datepicker_end.Date) || (datepicker_start.Date == datepicker_end.Date))
                {
                    DisplayAlert("Date Error", "Starting Date can't be after or equal to end date", "OK");
                    return;
                }

                if (string.IsNullOrEmpty(entry_course_name.Text) ||
                    string.IsNullOrEmpty(entry_instructor_name.Text) ||
                    string.IsNullOrEmpty(entry_instructor_phone.Text) ||
                    string.IsNullOrEmpty(entry_instructor_email.Text))
                {
                    DisplayAlert("Invalid Input", "Field(s) must not be empty", "OK");
                    return;
                }

                if (edit_mode)
                {
                    _course.CourseName = entry_course_name.Text;
                    _course.Status = pickerList[picker_status.SelectedIndex];
                    _course.StartDate = datepicker_start.Date;
                    _course.EndDate = datepicker_end.Date;
                    _course.Instructor = entry_instructor_name.Text;
                    _course.InstructorPhone = entry_instructor_phone.Text;
                    _course.InstructorEmail = entry_instructor_email.Text;
                    _course.Notification = switch_notification.IsToggled;
                    _course.Notes = entry_notes.Text;
                    conn.Update(_course);

                    Navigation.PopAsync();
                }
                else
                {
                    Course new_course = new Course();
                    new_course.Term = termId;
                    new_course.CourseName = entry_course_name.Text;
                    new_course.Status = pickerList[picker_status.SelectedIndex];
                    new_course.StartDate = datepicker_start.Date;
                    new_course.EndDate = datepicker_end.Date;
                    new_course.Instructor = entry_instructor_name.Text;
                    new_course.InstructorPhone = entry_instructor_phone.Text;
                    new_course.InstructorEmail = entry_instructor_email.Text;
                    new_course.Notification = switch_notification.IsToggled;
                    new_course.Notes = entry_notes.Text;
                    conn.Insert(new_course);

                    Navigation.PopAsync();
                }
            }
        }
    }
}