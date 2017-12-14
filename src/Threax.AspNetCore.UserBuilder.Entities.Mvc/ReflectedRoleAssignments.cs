using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;
using Threax.AspNetCore.Models;

namespace Threax.AspNetCore.UserBuilder.Entities.Mvc
{
    [UiTitle("Role Assignments")]
    public class ReflectedRoleAssignments : IRoleAssignments
    {
        [HiddenUiType]
        public Guid UserId { get; set; }

        [Display(Name = "Name")]
        [UiOrder(-9000000)]
        public string Name { get; set; }

        [Display(Name = "Edit Roles")]
        [UiOrder(9000000)]
        public bool EditRoles { get; set; }

        [Display(Name = "Super Admin")]
        [UiOrder(9000000)]
        public bool SuperAdmin { get; set; }

        public IEnumerable<Tuple<string, bool>> GetRoleValues()
        {
            foreach (var property in this.GetType().GetTypeInfo().GetProperties())
            {
                if (property.PropertyType == typeof(bool))
                {
                    yield return Tuple.Create<String, bool>(property.Name, (bool)property.GetValue(this));
                }
            }
        }

        public void SetRoleValues(IEnumerable<string> roles)
        {
            foreach (var property in this.GetType().GetTypeInfo().GetProperties())
            {
                if (property.PropertyType == typeof(bool))
                {
                    property.SetValue(this, roles.Contains(property.Name));
                }
            }
        }
    }
}