function renderAttendanceData(logs, aggregatedlogs){
       var month =["Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec","Jan","Feb","Mar"];
       var m=4;
      for(i=1;i<=12;i++)
      {
          var row='<tr><td colspan="3">'+month[i-1]+'</td>';
         
          for(j=1;j<=31;j++)
          {
            row=row+',<td id="m'+m+'d'+j+'"></td>';
          }
         
          row=row+'<td id="mt'+m+'"></td><td id="mp'+m+'"></td></tr>';
           m++;
           if(m>12)
            m=1;
       //   console.log(row);
        $("#attendance-tab").append(row);
      }
      
      $.each(logs,function(index,val){
          var prompt = val.date.split("-")
          var cdate = new Date(val.date);//prompt[2] , prompt[1] - 1, prompt[0]);
        
          if(val.state == 'p')
          $('#m'+(cdate.getMonth()+1)+'d'+cdate.getDate()+'').text("P");
          else if(val.state == 'l')
          $('#m'+(cdate.getMonth()+1)+'d'+cdate.getDate()+'').html("<span style='color:red;font-weight:bold;'>L</span>");
          else if(val.state == 'a')
          $('#m'+(cdate.getMonth()+1)+'d'+cdate.getDate()+'').html("<span style='color:red;font-weight:bold;'>A</span>");
          else if(val.state == 's')
          $('#m'+(cdate.getMonth()+1)+'d'+cdate.getDate()+'').html("<span style='color:blue;'>S</span>");

      });
      $.each(aggregatedlogs,function(index, val){
          $('#mt'+val.month+'').text(val.total);
          $('#mp'+val.month+'').text(val.percentage);
      });
}