namespace CRBVendas.Helpers;

public static class Formatador
{
    public static string CNPJ(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return "-";

        cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        if (cnpj.Length != 14)
            return cnpj;

        return $"{cnpj[..2]}.{cnpj.Substring(2,3)}.{cnpj.Substring(5,3)}/{cnpj.Substring(8,4)}-{cnpj.Substring(12,2)}";
    }

    public static string IE(string? ie)
    {
        if (string.IsNullOrWhiteSpace(ie))
            return "-";

        ie = new string(ie.Where(char.IsDigit).ToArray());

        if (ie.Length != 12)
            return ie;

        return $"{ie[..3]}.{ie.Substring(3,3)}.{ie.Substring(6,3)}.{ie.Substring(9,3)}";
    }

    public static string Telefone(string? tel)
    {
        if (string.IsNullOrWhiteSpace(tel))
            return "-";

        tel = new string(tel.Where(char.IsDigit).ToArray());

        if (tel.Length == 11)
            return $"({tel[..2]}) {tel.Substring(2,5)}-{tel.Substring(7)}";

        if (tel.Length == 10)
            return $"({tel[..2]}) {tel.Substring(2,4)}-{tel.Substring(6)}";

        return tel;
    }

    public static string CEP(string? cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return "-";

        cep = new string(cep.Where(char.IsDigit).ToArray());

        if (cep.Length != 8)
            return cep;

        return $"{cep[..5]}-{cep.Substring(5)}";
    }
}