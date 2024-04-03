namespace RatingStudents;

public static class WindowManager
{
    public static Window_Subjects windowSubjects;
    public static Window_Ratings windowRatings;
    public static Window_Students windowStudents;
    
    public static void StudentsManagerWindow(string openOrClose)
    {
        if (openOrClose.ToLower() == "close")
        {
            windowSubjects.MiWindowStudents.IsEnabled = true;
            windowRatings.MiWindowStudents.IsEnabled = true;
        }
        else
        {
            windowSubjects.MiWindowStudents.IsEnabled = false;
            windowRatings.MiWindowStudents.IsEnabled = false;
        }
    }
    public static void SubjectsManagerWindow(string openOrClose)
    {
        if (openOrClose.ToLower() == "close")
        {
            windowStudents.MiWindowSubjects.IsEnabled = true;
            windowRatings.MiWindowSubjects.IsEnabled = true;
        }
        else
        {
            windowStudents.MiWindowSubjects.IsEnabled = false;
            windowRatings.MiWindowSubjects.IsEnabled = false;
        }
    }
    public static void RatingsManagerWindow(string openOrClose)
    {
        if (openOrClose.ToLower() == "close")
        {
            windowStudents.MiWindowRatings.IsEnabled = true;
            windowSubjects.MiWindowRatings.IsEnabled = true;
        }
        else
        {
            windowStudents.MiWindowRatings.IsEnabled = false;
            windowSubjects.MiWindowRatings.IsEnabled = false;
        }
    }
}