namespace FsGraphqlDemo;

internal static class ShowInfo
{
    public static void ShowUserInfo(List<IStudentinfo> results, string? filter)
    {
        int count = 0;
        foreach (var result in results)
        {
            var student = result as IStudentinfo;
            ++count;
            ArgumentNullException.ThrowIfNull(student);
            ArgumentNullException.ThrowIfNull(student.Navn);
            ArgumentNullException.ThrowIfNull(student.PersonProfil);

            if (filter is not null && (student.PersonProfil.Fodselsnummer != filter && student.Studentnummer != filter))
            {
                continue;
            }
            string name = $"{student.Navn.Fornavn} {student.Navn.Etternavn}";
            Console.WriteLine($"[{count}] {name,-35}  Feide={student.FeideBruker}  Fnr={student.PersonProfil.Fodselsnummer}  Snr={student.Studentnummer}  Ltid={student?.LanetakerId}");
        }
    }

    public static void ShowUserInfo2(List<IStudentcardinfo> results)
    {
        int count = 0;
        foreach (var result in results)
        {
            var student = result as IStudentcardinfo;
            ++count;
            ArgumentNullException.ThrowIfNull(student);
            ArgumentNullException.ThrowIfNull(student.Navn);
            ArgumentNullException.ThrowIfNull(student.PersonProfil);

            string name = $"{student.Navn.Fornavn} {student.Navn.Etternavn}";
            Console.WriteLine($"[{count}] {name,-35}  Feide={student.FeideBruker}  Fnr={student.PersonProfil.Fodselsnummer}  Snr={student.Studentnummer}  Ltid={student?.LanetakerId}");
            foreach (var card in student?.Studentkort ?? [])
            {
                Console.WriteLine($"  studentkort: {card?.Studentkortnummer,-10} Periode: {card?.Gyldighetsperiode?.FraDato} -> {card?.Gyldighetsperiode?.TilDato}");
            }
        }
    }
}