using System;

namespace System
{
	public static class TimeUtils
	{
		/// <summary>
		/// Multiplies a time period by a number
		/// </summary>
		/// <param name="duration"></param>
		/// <param name="modifier"></param>
		/// <returns></returns>
		public static TimeSpan Multiply(this TimeSpan duration, float modifier)
		{
			if (modifier == 1) return duration;
			try
			{
				return TimeSpan.FromSeconds(duration.TotalSeconds*modifier);
			}
			catch(OverflowException ex)
			{
				if (ex.Message.Contains("long"))return TimeSpan.MaxValue;

				return TimeSpan.MinValue;
			}
		}

        public static TimeSpan ToDays(this int value)
        {
            return TimeSpan.FromDays(value);
        }

        public static TimeSpan ToMiliseconds(this int value)
        {
            return TimeSpan.FromMilliseconds(value);
        }

        public static TimeSpan ToSeconds(this int value)
        {
            return TimeSpan.FromSeconds(value);
        }

        /// <summary>
        /// Outputs the 'human friendly' format (ex: 4 days ago). English only
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToHuman(this TimeSpan time)
        {
            if (time.TotalDays>=365)
            {
                return Math.Round(time.TotalDays/356).ToString()+" years ago";
            }
            if (time.TotalDays>=30)
            {
                return Math.Round(time.TotalDays/30).ToString()+ " months ago";
            }

            if (time.TotalDays>7)
            {
                return Math.Round(time.TotalDays/7).ToString() + " weeks ago";
            }
            if (time.TotalDays>=1)
            {
                return time.Days.ToString() + " days ago";
            }

            if (time.TotalHours>=1)
            {
                return time.Hours.ToString() + " hours ago";
            }

            if (time.TotalMinutes>=1)
            {
                return time.Minutes.ToString() + " minutes ago";
            }
            return "few seconds ago";
        }
	}
}