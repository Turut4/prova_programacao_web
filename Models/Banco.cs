using System.Text.Json;


namespace Prova.Models;

public class Banco<Tmodel> : IBanco<Tmodel> where Tmodel : ModelBase
{
    private readonly string arquivo;
    private readonly List<Tmodel> Dados = new List<Tmodel>();

    public Banco()
    {
        string pasta = Path.Combine(
            Directory.GetCurrentDirectory(),
             "wwwroot", "dados"
        );

        if (!Directory.Exists(pasta))
        {
            Directory.CreateDirectory(pasta);
        }

        arquivo = Path.Combine(
            pasta,
            $"{typeof(Tmodel).Name}.json"
        );


        if (File.Exists(arquivo))
        {
            string json = File.ReadAllText(arquivo);

            Dados = JsonSerializer.Deserialize<List<Tmodel>>(json) ?? new List<Tmodel>();
        }
        else
            Dados = new List<Tmodel>();
    }

    private void SalvarDados()
    {
        string json = System.Text.Json.JsonSerializer.Serialize(Dados);

        File.WriteAllText(arquivo, json);
    }

    public void Adicionar(Tmodel model)
    {
        Dados.Add(model);
        SalvarDados();
    }

    public void Alterar(int id, Tmodel model)
    {
        var bdModel = Dados.FirstOrDefault(p =>
            p.Id == id);
        if (bdModel != null)
        {
            Dados.Remove(bdModel);
            Dados.Add(model);
            SalvarDados();
        }
    }

    public void Excluir(int id)
    {
        var bdModel = Dados.FirstOrDefault(p =>
            p.Id == id);
        if (bdModel != null)
        {
            Dados.Remove(bdModel);
            SalvarDados();
        }
    }

    public List<Tmodel> Listar()
    {
        return Dados.ToList();
    }

    public Tmodel? Busca(int id)
    {
        return Dados.FirstOrDefault(p =>
            p.Id == id);
    }
}
