
$(document).ready(function () {
    var ID = $("#IdPedido").val();
    buscar_estilos(ID);
});

function probar() {
    $('#tabless tr').on('click', function (e) {
        $('#tabless tr').removeClass('highlighted');
        $(this).addClass('highlighted');
    });
}


$(document).on("input", ".numeric", function () {
    this.value = this.value.replace(/\D/g, '');
});



//Registrar Batch

  function  registrarBatch(){
        debugger
        var nColumnas = $("#tablePrint tr:last td").length;

        var r = 0; var c = 0; var i = 0; var cadena = new Array(nColumnas - 1);
        for (var x = 0; x < nColumnas - 1; x++) {
            cadena[x] = '';
        }
        var nFilas = $("#tablePrint tbody>tr").length;
        r = 0;
        $('#tablePrint tbody>tr').each(function () {
            if (r > 0) {
                c = 0;
                $(this).find("input").each(function () {
                    $(this).closest('td').find("input").each(function () {
                        cadena[c] += this.value + "*";
                        c++;
                    });


                });
            }
            r++;
        });
        var error = 0;
        $('#tablePrint').find('td').each(function (i, el) {

            var valor = $(el).children().val();

            if ($(el).children().val() === '') {
                error++;
                $(el).children().css('border', '2px solid #e03f3f');

            } else {

                $(el).children().css('border', '');
            }



        });

      enviarListaTallaPrintShop(cadena, error);
    }

var estiloId;
function enviarListaTallaPrintShop(cadena, error) {
    var idTurno = $("#PrintShopC_Turnos option:selected").val();
    if (error !== 0) {
        var alert = alertify.alert("Message", 'All fields are required.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
    } else {
        $.ajax({
            url: "/PrintShop/Obtener_Lista_Tallas_PrintShop",
            datatType: 'json',
            data: JSON.stringify({ ListTalla: cadena, TurnoID: idTurno, EstiloID: estiloId }),
            cache: false,
            type: 'POST',
            contentType: 'application/json',
            success: function (data) {
                alertify.set('notifier', 'position', 'top-right');
                alertify.notify('The batch was correctly registered.', 'success', 5, null);
                $('input[type="number"]').val('0');
                obtener_tallas_item(estiloId);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                showError(xhr.status, xhr.responseText);
                if (data.error === 1) {
                    alertify.notify('Check.', 'error', 5, null);
                }
            }
        });
    }

}


//Actualizar información de un batch
var batchID;
function actualizarBatch() {
    debugger
    var nColumnas = $("#tablePrint tr:last td").length;

    var r = 0; var c = 0; var i = 0; var cadena = new Array(nColumnas - 1);
    for (var x = 0; x < nColumnas - 1; x++) {
        cadena[x] = '';
    }
    var nFilas = $("#tablePrint tbody>tr").length;
    r = 0;
    $('#tablePrint tbody>tr').each(function () {
        if (r > 0) {
            c = 0;
            $(this).find("input").each(function () {
                $(this).closest('td').find("input").each(function () {
                    cadena[c] += this.value + "*";
                    c++;
                });


            });
        }
        r++;
    });
    var error = 0;
    $('#tablePrint').find('td').each(function (i, el) {

        var valor = $(el).children().val();

        if ($(el).children().val() === '') {
            error++;
            $(el).children().css('border', '2px solid #e03f3f');

        } else {

            $(el).children().css('border', '');
        }



    });

    enviarListaTallaBatchPrintShop(cadena, error,batchID);
}

function enviarListaTallaBatchPrintShop(cadena, error,batchID) {
    var idTurno = $("#PrintShopC_Turnos option:selected").val();
    if (error !== 0) {
        var alert = alertify.alert("Message", 'All fields are required.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
    } else {
        $.ajax({
            url: "/PrintShop/Actualizar_Lista_Tallas_Batch",
            datatType: 'json',
            data: JSON.stringify({
                ListTalla: cadena, TurnoID: idTurno, EstiloID: estiloId, IdBatch: batchID}),
            cache: false,
            type: 'POST',
            contentType: 'application/json',
            success: function (data) {
                alertify.set('notifier', 'position', 'top-right');
                alertify.notify('The batch was modified correctly.', 'success', 5, null);
                $('input[type="number"]').val('0');
                obtener_tallas_item(estiloId);

            },
            error: function (xhr, ajaxOptions, thrownError) {
                showError(xhr.status, xhr.responseText);
                if (data.error === 1) {
                    alertify.notify('Check.', 'error', 5, null);
                }
            }
        });
    }

}


/*$(document).on('click', '#registarBatch', function () {
    obtenerTallas_PrintShop(estiloId);
});*/

function buscar_estilos(ID) {
    var tempScrollTop = $(window).scrollTop(); 
    $.ajax({
        url: "/Pedidos/Lista_Estilos_PO/" + ID,
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var html = '';
            var lista_estilo = jsonData.Data.listaItem;

            $.each(lista_estilo, function (key, item) {
                html += '<tr  onclick="probar()">';
                html += '<td>' + item.EstiloItem + '</td>';
                html += '<td>' + item.ItemDescripcion.Descripcion + '</td>';
                html += '<td>' + item.CatColores.CodigoColor + '</td>';
                html += '<td>' + item.CatColores.DescripcionColor + '</td>';
                html += '<td>' + item.Cantidad + '</td>';
                html += '<td>' + item.Price + '</td>';
                html += '<td>' + item.Total + '</td>';
                html += '<td><a href="#" onclick="obtener_tallas_item(' + item.IdItems + ');" class = "btn btn-default glyphicon glyphicon-search l1s" style = "color:black; padding:0px 5px 0px 5px;" Title = "Sizes"></a></td>';
                html += '</tr>';
            });
            if (Object.keys(lista_estilo).length === 0) {
                html += '<tr class="odd"><td valign="middle" align="center" colspan="10" class="dataTables_empty">No styles were found for the PO.</td></tr>';

            }
            $('.tbody').html(html);
            $("#div_estilos_orden").css("visibility", "visible");
            $(window).scrollTop(tempScrollTop); 
        },
        error: function (errormessage) { alert(errormessage.responseText); }
    });
}
var listaPO;

function obtener_tallas_item(IdEstilo) {
    var tempScrollTop = $(window).scrollTop(); 
    $("#loading").css('display', 'inline');
    estiloId = IdEstilo;
    obtener_tallas_PO(IdEstilo);
    $.ajax({
        url: "/Pedidos/Lista_Tallas_Estilo/" + IdEstilo,
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var html = '';
            var estilos = jsonData.Data.estilos;
            if (estilos !== '') {
                $("#div_estilo").html("<h2>Item: " + estilos + "</h2>");
                $("#div_estilo").show();
            } else {
                $("#div_estilo").hide();
            }

            var lista_estilo = jsonData.Data.listaTalla;
            listaEstiloPO = lista_estilo;
            html += '<tr> <th>  </th>'
            $.each(lista_estilo, function (key, item) {

                html += '<th>' + item.Talla + '</th>';

            });
            html += '<th> Total </th>'
            html += '</tr><tr><td>PO Quantity</td>';
            var cantidadesPO = 0;
            var cadena_cantidades = "";
            $.each(lista_estilo, function (key, item) {

                html += '<td class="total" >' + item.Cantidad + '</td>';
                cantidadesPO += item.Cantidad;
                cadena_cantidades += "*" + item.Cantidad;
            });
            var cantidades_array = cadena_cantidades.split('*');
            html += '<td>' + cantidadesPO + '</td>';
            html += '</tr>';
            html += '</tr><tr><td>Staging Quantity</td>';
            var cantidades = 0;
            var lista_Staging = jsonData.Data.listTallaStaging;//listaStaging.length;
            if (lista_Staging.length === 0) {
                lista_Staging = lista_estilo;
            } else {
                lista_Staging
            }
            $.each(lista_Staging, function (key, item) {
                if (lista_Staging === 0) {
                    item.Cantidad = 0;
                    html += '<td>' + item.Cantidad + '</td>';
                } else {
                    html += '<td>' + item.Cantidad + '</td>';
                }

                cantidades += item.Cantidad;
            });
            html += '<td>' + cantidades + '</td>';
            html += '</tr>';
            html += '</tr><tr><td>PrintShop Quantity</td>';
            var cantidadesPrint = 0;
            var lista_Batch = jsonData.Data.listaTallasTotalBatch;
            var listaTBatch = 0;
            $.each(lista_Batch, function (key, item) {
                listaTBatch++;
            });
            if (listaTBatch === 0) {
                lista_Batch = lista_estilo;
            } else {
                lista_Batch
            }
            $.each(lista_Batch, function (key, item) {
                if (listaTBatch === 0) {
                    item = 0;
                    html += '<td>' + item + '</td>';
                } else {
                    html += '<td>' + item + '</td>';
                }

                cantidadesPrint += item;
            });
            html += '<td>' + cantidadesPrint + '</td>';
            html += '</tr>';
            html += '</tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+ Printed</td>';
            var cantidadesPrinted = 0;
            var lista_Batch_Printed = jsonData.Data.listaTallasTotalPBatch;
            var listaPBatch = 0;
            $.each(lista_Batch_Printed, function (key, item) {
                listaPBatch++;
            });
            if (listaPBatch === 0) {
                lista_Batch_Printed = lista_estilo;
            } else {
                lista_Batch_Printed
            }
            $.each(lista_Batch_Printed, function (key, item) {
                if (listaPBatch === 0) {
                    item = 0;
                    html += '<td>' + item + '</td>';
                } else {
                    html += '<td>' + item + '</td>';
                }

                cantidadesPrinted += item;
            });
            html += '</tr>';
            html += '</tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+ MisPrint</td>';
            var cantidadesMisPrintB = 0;
            var lista_Batch_MP = jsonData.Data.listaTallasTotalMBatch;
            var listaMPBatch = 0;
            $.each(lista_Batch_MP, function (key, item) {
                listaMPBatch++;
            });
            if (listaMPBatch === 0) {
                lista_Batch_MP = lista_estilo;
            } else {
                lista_Batch_MP
            }
            $.each(lista_Batch_MP, function (key, item) {
                if (listaMPBatch === 0) {
                    item = 0;
                    html += '<td>' + item + '</td>';
                } else {
                    html += '<td>' + item + '</td>';
                }

                cantidadesMisPrintB += item;
            });
            html += '</tr>';
            html += '</tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+ Defect</td>';
            var cantidadesDefectB = 0;
            var lista_Batch_Defect = jsonData.Data.listaTallasTotalDBatch;
            var listaDefBatch = 0;
            $.each(lista_Batch_Defect, function (key, item) {
                listaDefBatch++;
            });
            if (listaDefBatch === 0) {
                lista_Batch_Defect = lista_estilo;
            } else {
                lista_Batch_Defect
            }
            $.each(lista_Batch_Defect, function (key, item) {
                if (listaDefBatch === 0) {
                    item = 0;
                    html += '<td>' + item + '</td>';
                } else {
                    html += '<td>' + item + '</td>';
                }

                cantidadesDefectB += item;
            });
            html += '</tr>';
            html += '<tr><td>+/-</td>';
            var totales = 0;
            var i = 1;
            $.each(lista_Batch, function (key, item) {
                if (listaTBatch === 0) {
                    item = 0;
                }
                var resta = (parseFloat(cantidades_array[i]) - parseFloat(item))
                html += '<td >' + resta + '</td>';
                i++;
            });
            html += '</tr>';

            if (Object.keys(lista_estilo).length === 0) {
                html += '<tr class="odd"><td valign="middle" align="center" colspan="10" class="dataTables_empty">No sizes were found for the style.</td></tr>';

            }
            $('.tbodys').html(html);
            $("#consultaTalla").css("visibility", "visible");
            $("#div_estilo").css("visibility", "visible");
            $("#arte").css("visibility", "visible");
            obtenerImagenPNL(estilos);
            obtenerImagenArte(estilos);
            obtener_bacth_estilo(IdEstilo);
            obtenerTallas_PrintShop(IdEstilo);
            //obtenerIdEstilo(IdEstilo);
            $("#loading").css('display', 'none');
            $(window).scrollTop(tempScrollTop); 

        },
        error: function (errormessage) { alert(errormessage.responseText); }

    });
}

var size;
function obtener_bacth_estilo(IdEstilo) {
    debugger
    var tempScrollTop = $(window).scrollTop(); 
    $("#loading").css('display', 'inline');
    $.ajax({
        url: "/PrintShop/Lista_Batch_Estilo/" + IdEstilo,
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var html = '';

            var estilos = jsonData.Data.estilos;
            if (estilos !== '') {
                $("#div_estilo_batch").html("<h2>BATCH REVIEW</h2>");
                $("#div_estilo_batch").show();
            } else {
                //$("#div_estilo_batch").hide();
            }
            var lista_batch = jsonData.Data.listaTalla;
            var numBatch = lista_batch.length;
            if (numBatch === 0) {
               // $("#div_tabla_talla").hide();
            }
            html += '<tr> <th>   </th>';
            $.each(lista_batch, function (key, item) {
                size = item.Batch;
            });
            if (numBatch === 0) {
                $.each(size, function (key, item) {
                   // html += '<th>' + item.Talla + '</th>';
                });
            } else {
                $.each(size, function (key, item) {
                    html += '<th>' + item.Talla + '</th>';
                });
            }
           
            html += '<th> Total </th>';
            html += '<th> User </th>';
            html += '<th> Turn </th>';
            html += '<th> Actions </th>';
            html += '</tr>';


            $.each(lista_batch, function (key, item) {
                html += '<tr><td>Batch-' + item.IdBatch + '</td>';

                var cantidad = 0;
                $.each(item.Batch, function (key, i) {

                    html += '<td class="total" >' + i.Total + '</td>';
                    cantidad += i.Total;
                });
                html += '<td>' + cantidad + '</td>';
                html += '<td>' + item.NombreUsr + '</td>';
                if (item.TipoTurno === 1) {
                    html += '<td>First Turn</td>';
                } else {
                    html += '<td>Second Turn</td>';
                }
               
                html += '<td><a href="#" onclick="obtenerTallas_Batch(' + item.IdBatch + ',' + item.TipoTurno + ',' + item.IdPrintShop +  ');" class = "btn btn-default glyphicon glyphicon-search l1s" style = "color:black; padding:0px 5px 0px 5px;" Title = "Details Bacth"></a></td>';
                html += '</tr>';
                
                
            });
            if (Object.keys(lista_batch).length === 0) {
                html += '<tr class="odd"><td valign="middle" align="center" colspan="10" class="dataTables_empty">No batches were found for the style.</td></tr>';

            }
            $('.tbodyBatch').html(html);
            $("#div_estilo_batch").css("visibility", "visible");
            $("#loading").css('display', 'none');
            $(window).scrollTop(tempScrollTop); 

        },
        error: function (errormessage) { alert(errormessage.responseText); }
    });
    calcular_Restantes();
}

function obtener_tallas_PO(IdEstilo) {
    $.ajax({
        url: "/Pedidos/Lista_Tallas_Estilo/" + IdEstilo,
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var html = '';

            listaPO = jsonData.Data.listaTalla;
        },
        error: function (errormessage) { alert(errormessage.responseText); }

    });

}

function obtenerIdEstilo(IdEstilo) {
    $.ajax({
        url: "/Arte/ObtenerIdEstilo/" + IdEstilo,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        success: function () {
        },
        error: function (errormessage) { alert(errormessage.responseText); }

    });

}


function obtenerImagenPNL(nombreEstilo) {
    $('#imagenPNL').attr('src', '/Arte/ConvertirImagenPNLEstilo?nombreEstilo=' + nombreEstilo);  
}



function obtenerImagenArte(nombreEstilo) {
    $('#imagenArte').attr('src', '/Arte/ConvertirImagenArteEstilo?nombreEstilo=' + nombreEstilo);
}


function ConfirmRev(a) {
    alertify.confirm("Are you sure you want to modify the batch?", function (result) {
        actualizarBatch();
    }).set({
        title: "Confirmation"
    });
}
//Muestra el formulario de registro para las tallas correspondientes del batch
function obtenerTallas_PrintShop(idEstilo) {
    var tempScrollTop = $(window).scrollTop(); 
    $("#loading").css('display', 'inline');
    CalcularTotal();
    calcular_Restantes();
    $.ajax({
        url: "/Pedidos/Lista_Tallas_PrintShop_Estilo/" + idEstilo,
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var html = '';
            var estilos = jsonData.Data.estilos;
            if (estilos !== '') {
                $("#div_estilo_print").html("<h2> Register new batch</h2>");
                $("#div_estilo_print").show();
                $("#registarBatch").hide();
                $("#modificarBatch").hide();
                $("#guardarBatch").show();
            } else {
                $("#div_estilo_print").hide();
            }

            var lista_estilo = jsonData.Data.listaTalla;
            var list = lista_estilo.length;
            if (list === 0) {
                lista_estilo = listaEstiloPO;
            }
            html += '</tr><tr><td>PO </td>';
            var cantidadesPO = 0;
            $.each(listaPO, function (key, item) {

                html += '<td id="po"><input type="text" id="po" class="form-control cantPO"  value="' + item.Cantidad + '"/></td>';
                cantidadesPO += item.Cantidad;
            });
       
            html += '<th> QTY </th>';
            html += '<tr > <th>  </th>';
            //*************************************************
            var lista_estilo_Tallas = jsonData.Data.listaEstiloTallas;
            $.each(lista_estilo_Tallas, function (key, item) {

                html += '<td><input type="text" id="talla" class="form-control talla" value="' + item.Talla + '"/></td>';

            });

            html += '<th> Total </th>';
            html += '</tr><tr><td>Printed</td>';
            var cantidades = 0;

            $.each(lista_estilo_Tallas, function (key, item) {     
                    item.Printed = item.Talla;
                    item.Printed = 0;
                html += '<td><input type="number" id="cantidad" class="txt form-control print numeric" onChange="calcular_Printed()" value="' + item.Printed + '"/></td>';
            

                cantidades += item.Printed;
            });
            html += '<td><input type="number" id="totalP" class="form-control "  value="' + cantidades + '" readonly/></td>';
            html += '</tr><tr><td>MisPrint</td>';
            var misPrintCant = 0;
            $.each(lista_estilo_Tallas, function (key, item) {
             
                    item.MisPrint = item.Talla;
                    item.MisPrint = 0;
                    html += '<td ><input type="number" id="misprint" class="txt form-control mp" onChange="calcular_MisPrint()" value="' + item.MisPrint + '"/></td>';
               

                misPrintCant += item.MisPrint;
            });
            html += '<td><input type="number" id="totalM" class="form-control totalM" value="' + misPrintCant + '" readonly/></td>';
            html += '</tr><tr ><td class="dato">Defect</td>';
            var defectCant = 0;

            $.each(lista_estilo_Tallas, function (key, item) {
             
                    item.Defect = item.Talla;
                    item.Defect = 0;
                    html += '<td ><input type="number" id="defect" class="txt form-control def " onChange="calcular_Defect()" value="' + item.Defect + '"/></td>';
              

                defectCant += item.Defect;
            });


            html += '<td><input type="number" id="totalD" class="form-control totalD" value="' + defectCant + '" readonly/></td>';
            html += '</tr><tr ><td class="total">+/-</td>';
            var total = 0;
            $.each(lista_estilo_Tallas, function (key, item) {
                html += ' <div class="span7">';              
                    item.Defect = item.Talla;
                    item.Defect = 0;
                    html += '<td ><input type="number" id="falt" class="form-control totalFal" value="' + item.Defect + '" readonly/></td>';
             
                html += ' </div>';
                total = cantidades + misPrintCant + defectCant;
            });
            html += '<td><input type="number" id="totalF" class="form-control totalF" value="' + total + '" readonly/></td>';
            html += '</tr>';


            $('.tbodyprint').html(html);
            $("#div_estilo_print").css("visibility", "visible");
            $("#loading").css('display', 'none');
            $(window).scrollTop(tempScrollTop);

        },
        error: function (errormessage) { alert(errormessage.responseText); }
    });
}


function obtenerTallas_Batch(idBatch, idTurno, idPrintShop) {
   // var tempScrollTop = $(window).scrollTop(); 
    $("#PrintShopC_Turnos").val(idTurno);
   
    batchID = idBatch;
    var actionData = "{'idEstilo':'" + estiloId + "','idBatch':'" + idBatch + "'}";
    $.ajax({
        url: "/PrintShop/Lista_Tallas_PrintShop_IdEstilo_IdBatch/",
        type: "POST",
        data: actionData,
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var html = '';
            var estilos = jsonData.Data.estilos;
            if (estilos !== '') {
                $("#div_estilo_print").html("<h2> Details Batch </h2>");
                $("#div_estilo_print").show();
                $("#registarBatch").show();
                $("#guardarBatch").hide();
                $("#modificarBatch").show();
            } else {
                $("#div_estilo_print").hide();
            }

            var lista_estilo = jsonData.Data.listaTalla;
            var list = lista_estilo.length;
            if (lista_estilo === 0) {
                lista_estilo = listaEstiloPO;
            } else {
                lista_estilo

            }


            html += '</tr><tr><td>PO </td>';
            var cantidadesPO = 0;
            $.each(listaPO, function (key, item) {

                html += '<td id="po"><input type="text" id="po" class="form-control cantPO"  value="' + item.Cantidad + '"/></td>';
                cantidadesPO += item.Cantidad;
            });
            html += '<th> QTY </th>';
            html += '<tr > <th>  </th>';
            $.each(lista_estilo, function (key, item) {


                html += '<td><input type="text" id="talla" class="form-control talla" value="' + item.Talla + '"/></td>';

            });
            html += '<th> Total </th>';
            $.each(lista_estilo, function (key, item) {
                identificador = item.IdPrintShop;
              
            });
            html += '</tr><tr><td>Printed</td>';
            var cantidades = 0;         
            $.each(lista_estilo, function (key, item) {
                if (list === 0) {

                    item.Printed = 0;
                    html += '<td><input type="number" id="cantidad" class="txt form-control print" onChange="calcular_Printed()" value="' + item.Printed + '"/></td>';
                } else {

                    html += '<td><input type="number" id="cantidad" class="txt form-control print"  onChange="calcular_Printed()" value="' + item.Printed + '"/></td>';
                }

                cantidades += item.Printed;
            });
            html += '<td><input type="number" id="totalP" class="form-control "  value="' + cantidades + '" readonly/></td>';
            html += '</tr><tr><td>MisPrint</td>';
            var misPrintCant = 0;
            $.each(lista_estilo, function (key, item) {
                if (list === 0) {
                    item.MisPrint = 0;
                    html += '<td ><input type="number" id="misprint" class="txt form-control mp" onChange="calcular_MisPrint()" value="' + item.MisPrint + '"/></td>';
                } else {

                    html += '<td > <input type="number" id="misprint" class=" txt form-control mp" onChange="calcular_MisPrint()" value="' + item.MisPrint + '"/></td>';
                }

                misPrintCant += item.MisPrint;
            });
            html += '<td><input type="number" id="totalM" class="form-control totalM" value="' + misPrintCant + '" readonly/></td>';
            html += '</tr><tr ><td class="dato">Defect</td>';
            var defectCant = 0;
            $.each(lista_estilo, function (key, item) {
                if (list === 0) {
                    item.Defect = 0;
                    html += '<td ><input type="number" id="defect" class="txt form-control def " onChange="calcular_Defect()" value="' + item.Defect + '"/></td>';
                } else {

                    html += '<td ><input type="number" id="defect" class="txt form-control def" onChange="calcular_Defect()" value="' + item.Defect + '"/></td>';
                }

                defectCant += item.Defect;
            });


            html += '<td><input type="number" id="totalD" class="form-control totalD" value="' + defectCant + '" readonly/></td>';
            html += '</tr><tr ><td class="total">+/-</td>';
            var total = 0;
            $.each(lista_estilo, function (key, item) {
                html += ' <div class="span7">';
                if (list === 0) {
                    item.Defect = 0;
                    html += '<td ><input type="number" id="falt" class="form-control totalFalt" value="' + item.Defect + '" readonly/></td>';
                } else {
                    item.Defect = 0;
                    html += '<td ><input type="number" id="falt" class="form-control totalFalt" value="' + item.Defect + '" readonly/></td>';
                }
                html += ' </div>';
                total = cantidades + misPrintCant + defectCant;
            });
            html += '<td><input type="number" id="totalF" class="form-control totalF" value="' + total + '" readonly/></td>';
            html += '</tr>';


            $('.tbodyprint').html(html);
            $("#div_estilo_print").css("visibility", "visible");
            CalcularTotalBatch();
           // $(window).scrollTop(tempScrollTop); 
        },
        error: function (errormessage) { alert(errormessage.responseText); }
    });
}

