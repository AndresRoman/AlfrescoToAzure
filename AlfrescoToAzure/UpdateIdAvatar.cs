using AlfrescoToAzure.ServiceReference1;
using System;
using System.Data;

namespace AlfrescoToAzure
{
    class UpdateIdAvatar
    {
        readonly ServiceReference1.ServicioDALClient objCliente;
        string str_id;
        int int_ente;

        public UpdateIdAvatar(string str_id, int int_ente)
        {
            objCliente = new ServiceReference1.ServicioDALClient();
            this.str_id = str_id;
            this.int_ente = int_ente;
        }

        public int updateId()
        {
            try
            {
                objCliente.AddInParameter(new ParametersIn() { StrNameParameter = "str_id", dbType = DbType.String, objValue = str_id });
                objCliente.AddInParameter(new ParametersIn() { StrNameParameter = "int_ente", dbType = DbType.Int32, objValue = int_ente });

                objCliente.AddOutParameter(new ParametersOut() { StrNameParameter = "int_o_error", dbType = DbType.Int32 });
                objCliente.AddOutParameter(new ParametersOut() { StrNameParameter = "str_o_error", dbType = DbType.String });

                objCliente.ExecuteNonQuery("update_avatar_id", "meg_servicios");
                int? res = (int)objCliente.ParameterOut("int_o_error");

                objCliente.EmptyLists();
                objCliente.Close();

                return (int)res;
            }
            catch (Exception ex)
            {
                objCliente.Abort();
                throw new ArgumentNullException(ex.Source + " : " + ex.Message + " (" + ex.ToString() + ")");
            }
        }

        public static implicit operator int(UpdateIdAvatar updateIdAvatar)
        {
            return updateIdAvatar.updateId();
        }
    }
}
