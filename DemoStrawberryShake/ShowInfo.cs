namespace DemoStrawberryShake;

internal static class ShowInfo
{
    public static void ShowUserInfo(List<IStudentinfo> results, string? filter, string? studentId)
    {
        int count = 0;
        foreach (var result in results)
        {
            var student = result as IStudentinfo;
            ++count;
            ArgumentNullException.ThrowIfNull(student);
            ArgumentNullException.ThrowIfNull(student.PersonProfil);
            ArgumentNullException.ThrowIfNull(student.PersonProfil.Navn);

            if (filter is not null && (student.PersonProfil.Fodselsnummer != filter && student.Studentnummer != filter))
            {
                continue;
            }
            if (!string.IsNullOrEmpty(studentId) && student.Id != studentId)
            {
                continue;
            }
            string name = $"{student.PersonProfil.Navn.Fornavn} {student.PersonProfil.Navn.Etternavn}";
            Console.WriteLine($"[{count}] {name,-35}  Feide={student.PersonProfil.FeideBruker,-16}  Fnr={student.PersonProfil.Fodselsnummer}  Snr={student.Studentnummer}  Ltid={student?.LanetakerId}  Id={student.Id}");
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
            ArgumentNullException.ThrowIfNull(student.PersonProfil);
            ArgumentNullException.ThrowIfNull(student.PersonProfil.Navn);

            string name = $"{student.PersonProfil.Navn.Fornavn} {student.PersonProfil.Navn.Etternavn}";
            Console.WriteLine($"[{count}] {name,-35}  Feide={student.FeideBruker,-16}  Fnr={student.PersonProfil.Fodselsnummer}  Snr={student.Studentnummer}  Ltid={student?.LanetakerId}");
            foreach (var card in student?.Studentkort ?? [])
            {
                Console.WriteLine($"  studentkort: {card?.Studentkortnummer,-10} Periode: {card?.Gyldighetsperiode?.FraDato} -> {card?.Gyldighetsperiode?.TilDato}");
            }
        }
    }
}