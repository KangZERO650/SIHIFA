namespace SIHIFA.Models;

public static class MediaConstants
{
    public static class Types
    {
        public const string Anime = "Anime";
        public const string Film = "Film";
        public const string Musik = "Musik";
        public const string Buku = "Buku";
        public const string Materi = "Materi";

        public static readonly string[] All = [Anime, Film, Musik, Buku, Materi];
    }

    public static class Statuses
    {
        public const string PlanTo = "Plan to Watch/Read/Listen";
        public const string Ongoing = "Sedang Dinikmati";
        public const string Completed = "Selesai (Completed)";
        public const string OnHold = "Ditunda (On Hold)";
        public const string Dropped = "Ditinggalkan (Dropped)";

        public static readonly string[] All = [PlanTo, Ongoing, Completed, OnHold, Dropped];
    }

    public static class Genres
    {
        public static readonly string[] Common =
        [
            "Action", "Adventure", "Comedy", "Drama", "Fantasy",
            "Sci-Fi", "Mystery", "Romance", "Slice of Life", "Thriller",
            "Horror", "Documentary", "Educational", "Pop", "Rock",
            "Classical", "Jazz", "Self-Improvement", "Teknologi"
        ];
    }
}
