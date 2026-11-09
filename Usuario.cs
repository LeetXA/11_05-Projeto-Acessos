using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _11_05___Projeto_Acessos
{
    internal class Usuario
    {
        private int id;
        private string nome;
        private List<Ambiente> ambientes;

        public int Id { get { return id; } }
        public string Nome { get { return nome; } }
        public List<Ambiente> Ambientes { get { return ambientes; } }

        public Usuario(int id, string nome)
        {
            this.id = id;
            this.nome = nome;
            ambientes = new List<Ambiente>();
        }

        public bool concederPermissao(Ambiente ambiente)
        {
            if (ambiente == null)
                return false;

            // Impede permissões duplicadas para o mesmo ambiente
            if (ambientes.Any(a => a.Id == ambiente.Id))
                return false;

            ambientes.Add(ambiente);
            return true;
        }

        public bool revogarPermissao(Ambiente ambiente)
        {
            if (ambiente == null)
                return false;

            Ambiente encontrado = ambientes.FirstOrDefault(
                a => a.Id == ambiente.Id
            );

            if (encontrado == null)
                return false;

            ambientes.Remove(encontrado);
            return true;
        }

        public bool possuiPermissao(Ambiente ambiente)
        {
            return ambientes.Any(a => a.Id == ambiente.Id);
        }
    }
}
