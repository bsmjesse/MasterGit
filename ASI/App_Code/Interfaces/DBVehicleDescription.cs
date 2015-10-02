using System;

namespace VLF.ASI.Interfaces
{
    /// <summary>
    /// WebMethod Descriptions for DBVehicle
    /// </summary>
    public static class DBVehicleDescription
    {

        public const string GetVehicleInfoXML = "<table><tr><!-- description --><td>Retrieves vehicle information by License Plate.</td></tr><tr><td style='padding-left: 25px; color: Blue; font-weight: bold;'>xml output:</td></tr><tr><td style='padding-left: 25px;'><table style='color: black; border: solid 1px silver;'><tr><td style='color: silver; width: 150px; border-left: solid 1px silver; border-bottom: solid 1px silver; text-align: center;'>field</td><td style='color: silver; width: 75px; border-left: solid 1px silver; border-bottom: solid 1px silver; text-align: center;'>datatype</td><td style='color: silver; width: 500px; border-left: solid 1px silver; border-bottom: solid 1px silver; text-align: center;'>info</td></tr><tr><td>licencePlate</td><td>string</td><td>&nbsp;</td></tr><tr><td>boxId</td><td>integer</td><td>&nbsp;</td></tr><tr><td>vehicleId</td><td>integer</td><td>&nbsp;</td></tr><tr><td>vinNum</td><td>string</td><td><b><i>V</i></b>ehicle <b><i>I</i></b>dentification <b><i>N</i></b>umber</td></tr><tr><td>makeModelId</td><td>integer</td><td>&nbsp;</td></tr><tr><td>makeName</td><td>string</td><td>&nbsp;</td></tr><tr><td>modelName</td><td>string</td><td>&nbsp;</td></tr><tr><td>vehicleTypeName</td><td>stringr</td><td>&nbsp;</td></tr><tr><td>stateProvince</td><td>string</td><td>&nbsp;</td></tr><tr><td>modelYear</td><td>string</td><td>&nbsp;</td></tr><tr><td>color</td><td>string</td><td>&nbsp;</td></tr><tr><td>description</td><td>string</td><td>&nbsp;</td></tr><tr><td>costPerMile</td><td>integer</td><td>&nbsp;</td></tr><tr><td>organizationId</td><td>integer</td><td>&nbsp;</td></tr><tr><td>iconTypeId</td><td>integer</td><td style='color:Red;'>* see <a href='#'>appendix 'e'</a></td></tr><tr><td>iconTypeName</td><td>string</td><td>&nbsp;</td></tr></table></td></tr></table>";

    }
}