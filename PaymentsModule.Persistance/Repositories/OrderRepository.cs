using Microsoft.EntityFrameworkCore;
using PaymentsModule.Domain.Interfaces.Repositories;
using PaymentsModule.Domain.Models;
using PaymentsModule.Persistance.Data;
using PaymentsModule.Persistance.Entities;

namespace PaymentsModule.Persistance.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly PaymentsDbContext _context;

    public OrderRepository(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Orders.FindAsync(id);
        return entity != null ? MapToModel(entity) : null;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        var entities = await _context.Orders.ToListAsync();
        return entities.Select(MapToModel);
    }

    public async Task<Order> CreateAsync(Order order)
    {
        var entity = MapToEntity(order);
        entity.Id = Guid.NewGuid();

        _context.Orders.Add(entity);
        await _context.SaveChangesAsync();

        return MapToModel(entity);
    }

    public async Task<Order?> UpdateAsync(Guid id, Order order)
    {
        var entity = await _context.Orders.FindAsync(id);
        if (entity == null) return null;

        entity.Status = order.Status;

        await _context.SaveChangesAsync();
        return MapToModel(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.Orders.FindAsync(id);
        if (entity == null) return false;

        _context.Orders.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static Order MapToModel(OrderEntity entity)
    {
        return new Order
        {
            Id = entity.Id,
            Status = entity.Status
        };
    }

    private static OrderEntity MapToEntity(Order model)
    {
        return new OrderEntity
        {
            Id = model.Id,
            Status = model.Status
        };
    }
}