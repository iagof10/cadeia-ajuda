using System.ComponentModel.DataAnnotations;

namespace CadeiaAjuda.Web.Services;

public class AreaApiClient
{
    // Dados em memória para desenvolvimento do front-end.
    // Será substituído por chamadas HTTP quando o back-end estiver pronto.
    private static readonly List<AreaViewModel> _areas = GenerateSampleData();

    public Task<List<AreaViewModel>> GetAllAsync()
        => Task.FromResult(_areas.ToList());

    public Task<AreaViewModel?> GetByIdAsync(Guid id)
        => Task.FromResult(_areas.FirstOrDefault(a => a.Id == id));

    public Task<HttpResponseMessage> CreateAsync(AreaFormModel model)
    {
        var area = new AreaViewModel
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            ParentId = model.ParentId,
            Active = model.Active,
            CreatedAt = DateTime.UtcNow
        };
        _areas.Add(area);
        return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.Created));
    }

    public Task<HttpResponseMessage> UpdateAsync(Guid id, AreaFormModel model)
    {
        var area = _areas.FirstOrDefault(a => a.Id == id);
        if (area is not null)
        {
            area.Name = model.Name;
            area.Description = model.Description;
            area.ParentId = model.ParentId;
            area.Active = model.Active;
        }
        return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
    }

    public Task<HttpResponseMessage> ToggleActiveAsync(Guid id)
    {
        var area = _areas.FirstOrDefault(a => a.Id == id);
        if (area is not null)
            area.Active = !area.Active;
        return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
    }

    public Task<HttpResponseMessage> DeleteAsync(Guid id)
    {
        var area = _areas.FirstOrDefault(a => a.Id == id);
        if (area is not null)
            _areas.Remove(area);
        return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
    }

    private static List<AreaViewModel> GenerateSampleData()
    {
        var fabricaId = Guid.NewGuid();
        var planta1Id = Guid.NewGuid();
        var planta2Id = Guid.NewGuid();
        var linha1Id = Guid.NewGuid();
        var linha2Id = Guid.NewGuid();

        return
        [
            new() { Id = fabricaId, Name = "Fábrica São Paulo", Description = "Unidade principal", Active = true, CreatedAt = DateTime.UtcNow },
            new() { Id = planta1Id, Name = "Planta A", Description = "Planta de montagem", ParentId = fabricaId, Active = true, CreatedAt = DateTime.UtcNow },
            new() { Id = planta2Id, Name = "Planta B", Description = "Planta de pintura", ParentId = fabricaId, Active = true, CreatedAt = DateTime.UtcNow },
            new() { Id = linha1Id, Name = "Linha 1", Description = "Linha de montagem principal", ParentId = planta1Id, Active = true, CreatedAt = DateTime.UtcNow },
            new() { Id = linha2Id, Name = "Linha 2", Description = "Linha de montagem secundária", ParentId = planta1Id, Active = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Posto 1A", Description = "Posto de montagem do motor", ParentId = linha1Id, Active = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Posto 1B", Description = "Posto de montagem do chassi", ParentId = linha1Id, Active = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Posto 2A", Description = "Posto de acabamento", ParentId = linha2Id, Active = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Cabine de Pintura 1", Description = "Cabine principal", ParentId = planta2Id, Active = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Cabine de Pintura 2", Description = "Cabine secundária", ParentId = planta2Id, Active = false, CreatedAt = DateTime.UtcNow },
        ];
    }
}

public class AreaViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AreaFormModel
{
    [Required(ErrorMessage = "Informe o nome da área.")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Guid? ParentId { get; set; }

    public bool Active { get; set; } = true;
}
