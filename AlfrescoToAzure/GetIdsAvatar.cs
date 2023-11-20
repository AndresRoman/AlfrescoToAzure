using AlfrescoToAzure.ServiceReference1;
using System;
using System.Data;

namespace AlfrescoToAzure
{
    class GetIdsAvatar
    {
        readonly ServiceReference1.ServicioDALClient objCliente;

        public GetIdsAvatar()
        {
            objCliente = new ServiceReference1.ServicioDALClient();
        }

        public DataTable obtenerIds()
        {
            
            try
            {

                //Datos requeridos                
                objCliente.AddOutParameter(new ParametersOut() { StrNameParameter = "int_o_error", dbType = DbType.Int32 });
                objCliente.AddOutParameter(new ParametersOut() { StrNameParameter = "str_o_error", dbType = DbType.String });

                DataSet ds = objCliente.ExecuteDataSet("get_avatar_ids", "meg_servicios");

                //Variables de salida
                string str_error_cod = objCliente.ParameterOut("int_o_error").ToString().Trim().PadLeft(3, '0');
                string str_error = (string)objCliente.ParameterOut("str_o_error");

                DataTable dtt_ids = new DataTable();
                dtt_ids = ds.Tables[0];

                objCliente.EmptyLists();
                objCliente.Close();

                return dtt_ids;
            }
            catch (Exception ex)
            {
                objCliente.Abort();
                throw new ArgumentNullException(ex.Source + " : " + ex.Message + " (" + ex.ToString() + ")");
            }
        }

        public static implicit operator DataTable(GetIdsAvatar getIds)
        {
            return getIds.obtenerIds();
        }
    }
}
