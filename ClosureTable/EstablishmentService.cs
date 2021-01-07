using ClosureTable.Models;
using ClosureTable.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClosureTable
{
    public class EstablishmentService
    {
        public async Task<List<Establishment>> GetAll()
        {
            using (var ctx = new AppDbContext())
            {
                return await ctx.Establishments.ToListAsync();
            }
        }

        public async Task<List<EstablishmentNode>> GetAllNodes()
        {
            using (var ctx = new AppDbContext())
            {
                return await ctx.EstablishmentNodes.ToListAsync();
            }
        }

        /// <summary>
        /// Insert new establishment, and its related nodes
        /// </summary>
        /// <param name="establishment"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public async Task<List<EstablishmentNode>> Insert(Establishment establishment, Establishment parent)
        {
            using (var ctx = new AppDbContext())
            {
                ctx.Establishments.Add(establishment);
                await ctx.SaveChangesAsync();

                ctx.EstablishmentNodes.Add(new EstablishmentNode()
                {
                    AncestorId = establishment.ID,
                    OffspringId = establishment.ID,
                    Separation = 0
                });
                await ctx.SaveChangesAsync();

                var nodes = await ctx.EstablishmentNodes
                    .SelectMany(t1 => ctx.EstablishmentNodes
                        .Where(t2 => t1.OffspringId == parent.ID && t2.AncestorId == establishment.ID)
                       .Select(t2 => new
                       {
                           AncestorId = t1.AncestorId,
                           OffspringId = t2.OffspringId,
                           Separation = t1.Separation + t2.Separation + 1
                       }).ToList())
                    .ToListAsync();

                var establishmentNodes = nodes.Select(x => new EstablishmentNode()
                {
                    AncestorId = x.AncestorId,
                    OffspringId = x.OffspringId,
                    Separation = x.Separation
                }).ToList();

                ctx.EstablishmentNodes.AddRange(establishmentNodes);
                await ctx.SaveChangesAsync();

                return await ctx.EstablishmentNodes.ToListAsync();
            }
        }

        /// <summary>
        /// Delete nodes and establishment
        /// </summary>
        /// <param name="establishment"></param>
        /// <returns></returns>
        public async Task Delete(Establishment establishment)
        {
            using (var ctx = new AppDbContext())
            {
                var offspringCount = await ctx.EstablishmentNodes.Where(x => x.AncestorId == establishment.ID).CountAsync();

                if (offspringCount > 1) // ignore seperation = 0
                {
                    throw new Exception("Establishment has child nodes. Delete the child nodes first.");
                }

                var nodes = await ctx.EstablishmentNodes
                    .Where(x => x.OffspringId == establishment.ID)
                    .ToListAsync();

                ctx.EstablishmentNodes.RemoveRange(nodes);
                
                var entry = ctx.Entry(establishment);
                if (entry.State == EntityState.Detached)
                    ctx.Establishments.Attach(establishment);
                ctx.Establishments.Remove(establishment);
                await ctx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Insert only nodes. Ignore Seperation = 0
        /// </summary>
        /// <param name="establishment"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public async Task InsertNodes(Establishment establishment, Establishment parent)
        {
            using (var ctx = new AppDbContext())
            {
                var nodes = await ctx.EstablishmentNodes
                    .SelectMany(t1 => ctx.EstablishmentNodes
                        .Where(t2 => t1.OffspringId == parent.ID && t2.AncestorId == establishment.ID)
                       .Select(t2 => new
                       {
                           AncestorId = t1.AncestorId,
                           OffspringId = t2.OffspringId,
                           Separation = t1.Separation + t2.Separation + 1
                       }).ToList())
                    .ToListAsync();

                var establishmentNodes = nodes.Select(x => new EstablishmentNode()
                {
                    AncestorId = x.AncestorId,
                    OffspringId = x.OffspringId,
                    Separation = x.Separation
                }).ToList();

                ctx.EstablishmentNodes.AddRange(establishmentNodes);
                await ctx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Delete only nodes. Ignore Seperation = 0
        /// </summary>
        /// <param name="establishment"></param>
        /// <returns></returns>
        public async Task DeleteNodes(Establishment establishment)
        {
            using (var ctx = new AppDbContext())
            {
                var offspringCount = await ctx.EstablishmentNodes.Where(x => x.AncestorId == establishment.ID).CountAsync();

                if (offspringCount > 1) // ignore seperation = 0
                {
                    throw new Exception("Establishment has child nodes. Delete the child nodes first.");
                }

                var nodes = await ctx.EstablishmentNodes
                    .Where(x => x.OffspringId == establishment.ID && x.Separation != 0)
                    .ToListAsync();

                ctx.EstablishmentNodes.RemoveRange(nodes);
                await ctx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Movement of leaf node to another leaf node
        /// </summary>
        /// <param name="establishment"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public async Task SimpleMove(Establishment establishment, Establishment parent)
        {
            await DeleteNodes(establishment);
            await InsertNodes(establishment, parent);
        }
    }
}
