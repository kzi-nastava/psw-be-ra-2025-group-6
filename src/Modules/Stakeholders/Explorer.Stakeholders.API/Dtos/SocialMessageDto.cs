using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos;

public class SocialMessageDto
{
    public long SenderId { get; set; }
    public long ReceiverId { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}
