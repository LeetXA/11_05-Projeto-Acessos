using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _11_05___Projeto_Acessos
{
    internal class Ambiente
    {
        private int id;
        private string nome;
        private Queue<Log> logs;

        public int Id { get { return id; } }
        public string Nome { get { return nome; } }
        public Queue<Log> Logs { get { return logs; } }

        public Ambiente(int id, string nome)
        {
            this.id = id;
            this.nome = nome;
            logs = new Queue<Log>();
        }

        public void registrarLog(Log log)
        {
            if (log == null)
                return;

            // Mantém no máximo 100 registros
            if (logs.Count >= 100)
            {
                logs.Dequeue();
            }

            logs.Enqueue(log);
        }
    }
}
