var month=new Array(12);
month[0]="January";
month[1]="February";
month[2]="March";
month[3]="April";
month[4]="May";
month[5]="June";
month[6]="July";
month[7]="August";
month[8]="September";
month[9]="October";
month[10]="November";
month[11]="December";

function FormatStringDate(CSharpDateString, Format) {
    if(Format === "dateTimeSmall"){
        return DateTimeToDateTimeSmall(GMTToLocalDate(CSharpDateStringToJSDate(CSharpDateString)));
    }
    else if(Format ==="dateTimeLarge"){
        return DateTimeToDateTimeLarge(GMTToLocalDate(CSharpDateStringToJSDate(CSharpDateString)));
    }
    else if(Format === "dateSmall"){
        return DateTimeToDateSmall(GMTToLocalDate(CSharpDateStringToJSDate(CSharpDateString)));
    }
    else if(Format === "dateLarge"){
        return DateTimeToDateLarge(GMTToLocalDate(CSharpDateStringToJSDate(CSharpDateString)));
    }
    else if(Format ==="time") {
        return DateTimeToTime(GMTToLocalDate(CSharpDateStringToJSDate(CSharpDateString)));
    }
}

function CSharpDateStringToJSDate(CSharpDateString)
{
    var JSDate = new Date(CSharpDateString);
    return JSDate;
}

function GMTToLocalDate(dateTime)
{
    localOffset = new Date().getTimezoneOffset() * -60000;
    var utcMs = dateTime.getTime();
    utcMs += localOffset;
    return new Date(utcMs);  
}

function DateTimeToTime(dateTimeParm)
{
    var dateTime = dateTimeParm;
    var minutes = dateTime.getMinutes() + 1;
    if(minutes < 10)
        minutes = "0" + minutes;
    var hours = dateTime.getHours();
    var hourTag = "AM";
    if(hours > 12)
    {
        hourTag = "PM";
        hours = hours -= 12;
    }
    return hours + ":" 
        + minutes + " "
        + hourTag;
}

function DateTimeToDateLarge(dateTimeParm)
{
    var dateTime = dateTimeParm;
    return month[dateTime.getMonth()] + " " 
        + (dateTime.getDate()) + ", " 
        + dateTime.getFullYear();;
}

function DateTimeToDateTimeSmall(dateTimeParm)
{
    var dateTime = dateTimeParm;
    var minutes = dateTime.getMinutes() + 1;
    var hours = dateTime.getHours();
    var hourTag = "AM";
    
    if(minutes < 10)
        minutes = "0" + minutes;
    if(hours > 12)
    {
        hourTag = "PM";
        hours = hours -= 12;
    }
        
    return (dateTime.getMonth() + 1) + "/" 
        + (dateTime.getDate()) + "/" 
        + dateTime.getFullYear() + " "
        + hours + ":" 
        + minutes + " "
        + hourTag;
}

function DateTimeToDateSmall(dateTimeParm)
{
    var dateTime = dateTimeParm;
    return (dateTime.getMonth() + 1) + "/" 
        + (dateTime.getDate()) + "/" 
        + dateTime.getFullYear();
}

function DateTimeToDateTimeLarge(dateTimeParm)
{
    var dateTime = dateTimeParm;
    var minutes = dateTime.getMinutes() + 1;
    if(minutes < 10)
        minutes = "0" + minutes;
    var hours = dateTime.getHours();
    var hourTag = "AM";
    if(hours > 12)
    {
        hourTag = "PM";
        hours = hours -= 12;
    }
    return month[dateTime.getMonth()] + " " 
        + (dateTime.getDate()) + ", " 
        + dateTime.getFullYear() + " "
        + hours + ":" 
        + minutes + " "
        + hourTag;
}

function MsToMinSecs(milliseconds) {
    var sec = Math.floor(milliseconds / 1000);
    var min = Math.floor(sec / 60);
    
    if(min == 0)
        return sec.toString() + "sec";
        
    return min.toString() + "min " + sec.toString() + "sec";;
}

function SecondsToHoursMinutes(seconds)
{
    var minutes = seconds / 60;
    var hours = minutes / 60;
    minutes = minutes % 60;
    minutes = Math.floor(minutes);
    hours = Math.floor(hours);
    if(hours == 0)
        return minutes.toString() + "min";
        
    if(minutes.length == 1)
        minutes = "0" + minutes;
    return hours.toString() + "h " + minutes.toString() + "min";;
}

function StatusIntegerToTest(integer) {
    if (integer == 0)
        return "Running";
    if (integer == 1)
        return "No Response";
    if (integer == 2)
        return "Not Found";
    if (integer == 3)
        return "QueryError";
    return "Error";
}