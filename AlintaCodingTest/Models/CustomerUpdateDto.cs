using System;
using System.ComponentModel.DataAnnotations;

namespace AlintaCodingTest.Models
{
    public class CustomerUpdateDto
    {
      //  public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
