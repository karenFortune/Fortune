$(document).ready(function () {
    var ID = $("#IdPedido").val();
    buscar_estilos(ID);
    $("#div_tabla_packing").css("visibility", "hidden");

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

//Autocomplete tallas
$(function () {
    var list_datalist = Array();
    $.ajax({
        url: '/Tallas/Lista_Tallas',
        type: 'GET',
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                var html = "";
                list_datalist.push(data[i].Talla);
            }
        }
    })
    var availableTags = list_datalist;
    $(document).on("focus keyup", "input.talla", function (event) {
        debugger
        $(this).autocomplete({
            source: availableTags,
            select: function (event, ui) {
                event.preventDefault();
                this.value = ui.item.label;
            },
            focus: function (event, ui) {
                event.preventDefault();
                this.value = ui.item.label;
            }
        });
    });
});

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
                html += '<td><a href="#" onclick="obtenerListaTallas(' + item.IdItems + ');" class = "btn btn-default glyphicon glyphicon-search l1s" style = "color:black; padding:0px 5px 0px 5px;" Title = "Sizes"></a></td>';
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

var estiloId;
var tipoEmp = "";
function obtenerListaTallas(EstiloId) {
    
    // $("#loading").css('display', 'inline');
    estiloId = EstiloId;
    $.ajax({
        url: "/Packing/Lista_Tallas_Por_Estilo/" + EstiloId,
        method: 'POST',
        dataType: "json",
        success: function (jsonData) {
            var listaT = jsonData.Data.listaTalla;
            var listaPacking = jsonData.Data.listaPackingS;
            var listaEmpaque = jsonData.Data.listaEmpaqueTallas;
            var listaTCajas = jsonData.Data.listaTotalCajasPack;
            var listaPO = jsonData.Data.lista;
            //var listaTCajas = jsonData.Data.listaCajasT;
            var html = '';
            var estilos = jsonData.Data.estilos;
            $("#btnAdd").hide();
            $("#nuevaTalla").hide();
               $("#div_estilo_ht").html("<h3>QUALITY OF SIZES</h3>");
                html += '<tr> <th width="30%"> Size </th>'
                $.each(listaPO, function (key, item) {
                    html += '<th>' + item.Talla + '</th>';
                });
                html += '<th width="30%"> Total </th>'
                html += '</tr><tr><td width="30%">1rst QTY</td>';
                var cantidades = 0;
            var cadena_cantidades = "";
                $.each(listaPO, function (key, item) {
                    html += '<td class="calidad">' + item.Cantidad + '</td>';
                    cantidades += item.Cantidad;
                    cadena_cantidades += "*" + item.Cantidad;
                });
                var cantidades_array = cadena_cantidades.split('*');
                html += '<td>' + cantidades + '</td>';

                if (listaEmpaque.length === 0) {
                    listaEmpaque;
                    html += '</tr><tr><td width="30%">Type Packing</td>';
                } else {
                    $.each(listaEmpaque, function (key, item) {
                        tipoEmp = item.NombreTipoPak;
                    });

                }
                var cantidadesEmp = 0;
                if (tipoEmp === "BULK") {
                    html += '</tr><tr><td width="30%">' + tipoEmp + '- #Pieces' + '</td>';

                    $.each(listaEmpaque, function (key, item) {
                        html += '<td>' + item.Pieces + '</td>';
                        cantidadesEmp += item.Pieces;
                    });
                } else if (tipoEmp === "PPK") {
                    html += '</tr><tr><td width="30%">' + tipoEmp + '- #Ratio' + '</td>';
                    $.each(listaEmpaque, function (key, item) {
                        html += '<td>' + item.Ratio + '</td>';
                        cantidadesEmp += item.Ratio;
                    });
                }
                html += '<td>' + cantidadesEmp + '</td>';
                html += '</tr><tr><td width="30%">Packages</td>';
                /*  $.each(listaTCajas, function (key, item) {
                      html += '<td>' + item.TotalPiezas + '</td>';
                  });*/
                var cantidadesTBox = 0;
                var lista_Batch = jsonData.Data.listaTallasTotalBatch;
                var listaTBatch = 0;
                $.each(listaTCajas, function (key, item) {
                    listaTBatch++;
                });
                if (listaTBatch === 0) {
                    listaTCajas = listaPacking;
                } else {
                    listaTCajas
                }
                $.each(listaTCajas, function (key, item) {
                    if (listaTBatch === 0) {
                        item = 0;
                        html += '<td>' + item + '</td>';
                    } else {
                        html += '<td>' + item + '</td>';
                    }

                    cantidadesTBox += item;
                });
                html += '<td>' + cantidadesTBox + '</td>';
                html += '</tr><tr>';
                if (tipoEmp === "BULK") {
                    html += '<td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+ Box</td>';
                }

                var lista_Batch_Box = jsonData.Data.listaCajasT;
                var listaPBatch = 0;
                $.each(lista_Batch_Box, function (key, item) {
                    listaPBatch++;
                });
                if (listaPBatch === 0) {
                    lista_Batch_Box = listaPacking;
                } else {
                    lista_Batch_Box
                }
                $.each(lista_Batch_Box, function (key, item) {

                    if (tipoEmp === "PPK") {
                        if (key === 1) {
                            if (listaPBatch === 0) {
                                item = 0;
                                html += '<td>' + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+ Box# -" + item + '</td>';
                            } else {
                                html += '<td>' + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+ Box# -" + item + '</td>';
                            }
                        }
                    } else {
                        if (listaPBatch === 0) {
                            item = 0;
                            html += '<td>' + item + '</td>';
                        } else {
                            html += '<td>' + item + '</td>';
                        }
                    }
                    // cantidadesPrinted += item;
                });
                html += '</tr><tr><td width="30%">+/-</td>';
                var totales = 0;
                var i = 1;
                $.each(listaTCajas, function (key, item) {
                    if (listaTBatch === 0) {
                        item = 0;
                    }
                    var resta = (parseFloat(cantidades_array[i]) - parseFloat(item))
                    html += '<td class="faltante">' + resta + '</td>';
                    i++;
                });
                html += '</tr>';

                $('.tbodyPHT').html(html);
                TallasEmpaqueBulkHT();

           
            $("#consultaTallaHT").css("visibility", "visible");
            $("#arte").css("visibility", "visible");
            obtenerImagenPNL(estilos);
            obtenerImagenArte(estilos);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        },
    }).done(function (data) {

    });
}

function TallasEmpaqueBulkHT() {
    $.ajax({
        url: "/Packing/Lista_Tallas_Empaque_HT_Por_Estilo/" + estiloId,
        method: 'POST',
        dataType: "json",
        success: function (jsonData) {
            var listaT = jsonData.Data.listaTalla;
            var listaPO = jsonData.Data.lista;
            var listaPacking = jsonData.Data.listaPackingS;
            var html = '';
            if (listaPacking.length === 0) {
                $("#btnAddP").show();
                $("#btnNuevoPPK").hide();
                $("#btnNuevo").prop("disabled", true);
                $("#btnNext").prop("disabled", true);
                $("#btnDone").prop("disabled", true);         
                $("#div_titulo_Bulk").html("<h3>REGISTRATION OF TYPE OF PACKAGING - BULK</h3>");
                $("#div_titulo_Bulk").css("visibility", "visible");
                $("#opciones").css("visibility", "visible");
                html += '<table class="table" id="tablaTallasBulkHT"><thead>';
                html += '<tr><th>Size</th>' +
                    ' <th>QTY#</th>' +
                    '</tr>' +
                    '</thead><tbody>';
                $.each(listaPO, function (key, item) {
                    html += '<tr>';
                    html += '<td width="20%"><input type="text" id="f-talla" class="form-control talla" value="' + item.Talla + '" readonly/></td>';
                    html += '<td width="20%"><input type="text" name="l-cantidad" id="l-cantidad" class="form-control numeric qty " value="' + 0 + '"  /></td>';
                   // html += '<td width="250"><button type="button" id="btnDelete" class="deleteTalla btn btn btn-danger btn-xs" value="4">Delete</button></td>';
                    html += '</tr>';
                });
                html += '</tbody> </table>';    
                html += '<button type="button" id="nuevoEmpaqueBulkHT" class="btn btn-success btn-md pull-right btn-sm"><span class="glyphicon glyphicon-floppy-disk" aria-hidden="true"></span> BULK</button>';               
                $('#listaTallaPHT').html(html);
            } else {
                $("#div_titulo_Bulk").html("<h3>PACKING DETAILS</h3>");
                $("#div_titulo_Bulk").css("visibility", "visible");
                html += '<tr> <th width="30%"> Size </th>'
                $.each(listaPacking, function (key, item) {
                    html += '<th width="30%">' + item.Talla + '</th>';
                });
                html += '</tr><tr><td width="30%">#Pieces</td>';
                var cantidades = 0;
                var cadena_cantidades = "";
                $.each(listaPacking, function (key, item) {
                    html += '<td width="30%">' + item.Pieces + '</td>';
                    cantidades += item.Pieces;
                    cadena_cantidades += "*" + item.Pieces;
                });
                html += '</tr>';
                $('.tbodyBulk').html(html);
            }
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        },
    }).done(function (data) {
    });
}

function ConfirmEmpaqueBulk(a) {
    var confirm = alertify.confirm('Confirmation', 'Do you want to register a new type of packaging Bulk ?', null, null).set('labels', { ok: 'Accept', cancel: 'Cancel' });
    confirm.set('closable', false); 
    confirm.set('onok', function () {
        //crearTallasBulk();
        limpiarFormBulk();
        $("#btnNuevo").prop("disabled", true);
        $("#btnNext").prop("disabled", true);
        $("#btnDone").prop("disabled", true);
        $("#nuevoEmpaqueBulkHT").prop("disabled", false);
       // alertify.success('The packaging was registered correctly.');
    });
    confirm.set('oncancel', function () { 

    });
}

function ConfirmEmpaquePPK(a) {
    var confirm = alertify.confirm('Confirmation', 'Do you want to register a new type of packaging PPK ?', null, null).set('labels', { ok: 'Accept', cancel: 'Cancel' });
    confirm.set('closable', false);
    confirm.set('onok', function () {
        //crearTallasBulk();
        limpiarFormPPK();
        $("#btnNuevoPPK").prop("disabled", true);
        $("#btnNext").prop("disabled", true);
        $("#btnDone").prop("disabled", true);
        $("#nuevoEmpaquePPKHT").prop("disabled", false);
        // alertify.success('The packaging was registered correctly.');
    });
    confirm.set('oncancel', function () {

    });
}

function limpiarFormBulk() {
    $('#tablaTallasBulkHT tbody>tr').each(function () {      
        $(this).find("input.qty").each(function () {
            $(this).closest('td').find("input.qty").each(function () {
              var valor= $(this).val(0);
            });        
        });
    });
    $('#Packing_PackingTypeSize_FormaEmpaque').val(0)
    $('#Packing_PackingTypeSize_NumberPO').val('');
}

function limpiarFormPPK() {
    $('#tablaTallasPPKHT tbody>tr').each(function () {
        $(this).find("input.qty").each(function () {
            $(this).closest('td').find("input.qty").each(function () {
                var valor = $(this).val(0);
            });
        });
    });
    //$('#Packing_PackingTypeSize_FormaEmpaque').val(0)
    $('#Packing_PackingTypeSize_NumberPO').val('');
}
//Registrar tallas Bulk HT
//function crearTallasBulk() {
    $(document).on("click", "#nuevoEmpaqueBulkHT", function () {
        var r = 0; var c = 0; var i = 0; var cadena = new Array(2);
        cadena[0] = ''; cadena[1] = '';
        var nFilas = $("#tablaTallasBulkHT tbody>tr").length;
        var nColumnas = $("#tablaTallasBulkHT tr:last td").length;
        $('#tablaTallasBulkHT tbody>tr').each(function () {
            r = 0;
            c = 0;
            $(this).find("input").each(function () {
                $(this).closest('td').find("input").each(function () {
                    cadena[c] += this.value + "*";
                    c++;
                });
                r++;
            });
        });
        var error = 0;
        $('#tablaTallasBulkHT').find('td').each(function (i, el) {
            var valor = $(el).children().val();
            if ($(el).children().val() === '') {
                error++;
                $(el).children().css('border', '2px solid #e03f3f');

            } else {
                $(el).children().css('border', '1px solid #cccccc');
            }
        });
        var formaEmpaque = $("#Packing_PackingTypeSize_FormaEmpaque option:selected").val();
        if (formaEmpaque === "0") {
            error++;
            $('#Packing_PackingTypeSize_FormaEmpaque').css('border', '2px solid #e03f3f');
        }
        else {
            $('#Packing_PackingTypeSize_FormaEmpaque').css('border', '');
        }

        var numberPO = $("#Packing_PackingTypeSize_NumberPO").val();
        if (numberPO === "" ) {
            error++;
            $('#Packing_PackingTypeSize_NumberPO').css('border', '2px solid #e03f3f');
        }
        else {
            $('#Packing_PackingTypeSize_NumberPO').css('border', '');
        }

        enviarListaTallaBulkHT(cadena, error);
    });
//}


function enviarListaTallaBulkHT(cadena, error) {
    var idFormaP = $("#Packing_PackingTypeSize_FormaEmpaque option:selected").val();
    var idNumberPo = $("#Packing_PackingTypeSize_NumberPO").val();
    if (error !== 0) {
        var alert = alertify.alert("Message", 'All fields are required.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
    } else {
        $.ajax({
            url: "/Packing/Obtener_Lista_Tallas_Packing_Bulk_HT",
            datatType: 'json',
            data: JSON.stringify({ ListTalla: cadena, EstiloID: estiloId, FormaPackID: idFormaP, NumberPOID: idNumberPo }),
            cache: false,
            type: 'POST',
            contentType: 'application/json',
            success: function (data) {
                alertify.set('notifier', 'position', 'top-right');
                alertify.notify('The type of packaging is registered correctly.', 'success', 5, null);
                $("#btnNuevo").prop("disabled", false);
                $("#btnNext").prop("disabled", false);
                $("#btnDone").prop("disabled", false);
                $("#nuevoEmpaqueBulkHT").prop("disabled", true);
                // obtenerListaTallas(estiloId);
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

$(document).on("click", "#btnNext", function () {
    $('#Packing_PackingTypeSize_NumberPO').val('');
    TallasEmpaquePPKHT();
});

function TallasEmpaquePPKHT() {
    $.ajax({
        url: "/Packing/Lista_Tallas_Empaque_HT_Por_Estilo/" + estiloId,
        method: 'POST',
        dataType: "json",
        success: function (jsonData) {
            var listaT = jsonData.Data.listaTalla;
            var listaPO = jsonData.Data.lista;
            var listaPacking = jsonData.Data.listaPackingS;
            var html = '';
            if (listaPacking.length === 0) {
                $("#btnAddP").show();
                $("#btnNuevoPPK").show();
                $("#btnNuevo").hide();
                $("#btnNuevo").prop("disabled", true);
                $("#btnNext").hide();
                $("#btnDone").prop("disabled", true);
                $("#btnNuevoPPK").prop("disabled", true);
                $('#Packing_PackingTypeSize_FormaEmpaque').hide();
                $('label[for="Packing_PackingTypeSize_FormaEmpaque"]').hide();
                $("#div_titulo_Bulk").html("<h3>REGISTRATION OF TYPE OF PACKAGING - PPK</h3>");
                $("#div_titulo_Bulk").css("visibility", "visible");
                $("#opciones").css("visibility", "visible");
                html += '<table class="table" id="tablaTallasPPKHT"><thead>';
                html += '<tr><th>Size</th>' +
                    ' <th>Ratio</th>' +
                    '</tr>' +
                    '</thead><tbody>';
                $.each(listaPO, function (key, item) {
                    html += '<tr>';
                    html += '<td width="250"><input type="text" id="f-talla" class="form-control talla" value="' + item.Talla + '" readonly/></td>';
                    html += '<td width="250"><input type="text" name="l-cantidad" id="l-cantidad" class="form-control numeric qty " value="' + 0 + '"  /></td>';
                  //  html += '<td width="250"><button type="button" id="btnDelete" class="deleteTalla btn btn btn-danger btn-xs" value="4">Delete</button></td>';
                    html += '</tr>';
                });
                html += '</tbody> </table>';
                html += '<button type="button" id="nuevoEmpaquePPKHT" class="btn btn-success btn-md pull-right btn-sm"><span class="glyphicon glyphicon-floppy-disk" aria-hidden="true"></span> PPK</button>';
                $('#listaTallaPHT').html(html);
            } else {
                $("#div_titulo").html("<h3>PACKING DETAILS</h3>");
                $('#Packing_PackingTypeSize_TipoEmpaque').hide();
                $.each(listaPacking, function (key, item) {
                    $('#Packing_PackingTypeSize_NombreTipoPak').val(item.NombreTipoPak);
                });

                $("#div_titulo").css("visibility", "visible");
                html += '<tr> <th width="30%"> Size </th>'
                $.each(listaPacking, function (key, item) {
                    html += '<th width="30%">' + item.Talla + '</th>';
                });
                html += '</tr><tr><td width="30%">#Ratio</td>';
                var cantidades = 0;
                var cadena_cantidades = "";
                $.each(listaPacking, function (key, item) {
                    html += '<td width="30%">' + item.Ratio + '</td>';
                    cantidades += item.Ratio;
                    cadena_cantidades += "*" + item.Ratio;
                });
                html += '</tr>';
                $('.tbodyBulk').html(html);
            }
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        },
    }).done(function (data) {
    });
}
//Registrar tallas PPK

$(document).on("click", "#nuevoEmpaquePPKHT", function () {
    var r = 0; var c = 0; var i = 0; var cadena = new Array(2);
    cadena[0] = ''; cadena[1] = '';
    var nFilas = $("#tablaTallasPPKHT tbody>tr").length;
    var nColumnas = $("#tablaTallasPPKHT tr:last td").length;
    $('#tablaTallasPPKHT tbody>tr').each(function () {
        r = 0;
        c = 0;
        $(this).find("input").each(function () {
            $(this).closest('td').find("input").each(function () {
                cadena[c] += this.value + "*";
                c++;
            });
            r++;
        });
    });
    var error = 0;
    $('#tablaTallasPPKHT').find('td').each(function (i, el) {
        var valor = $(el).children().val();
        if ($(el).children().val() === '' ) {
            error++;
            $(el).children().css('border', '2px solid #e03f3f');

        } else {
            $(el).children().css('border', '1px solid #cccccc');
        }
    });

    

    var numberPO = $("#Packing_PackingTypeSize_NumberPO").val();
    if (numberPO === "") {
        error++;
        $('#Packing_PackingTypeSize_NumberPO').css('border', '2px solid #e03f3f');
    }
    else {
        $('#Packing_PackingTypeSize_NumberPO').css('border', '');
    }

    enviarListaTallaPPKHT(cadena, error);
});


function enviarListaTallaPPKHT(cadena, error) {
    var idNumberPo = $("#Packing_PackingTypeSize_NumberPO").val();
    if (error !== 0) {
        var alert = alertify.alert("Message", 'All fields are required.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
    } else {
        $.ajax({
            url: "/Packing/Obtener_Lista_Tallas_Packing_PPK_HT",
            datatType: 'json',
            data: JSON.stringify({ ListTalla: cadena, EstiloID: estiloId, NumberPOID: idNumberPo }),
            cache: false,
            type: 'POST',
            contentType: 'application/json',
            success: function (data) {
                alertify.set('notifier', 'position', 'top-right');
                alertify.notify('The type of packaging is registered correctly.', 'success', 5, null);
                $("#btnNext").prop("disabled", false);
                $("#btnDone").prop("disabled", false);
                $("#btnNuevoPPK").prop("disabled", false);                
                $("#nuevoEmpaquePPKHT").prop("disabled", true);
                //obtenerListaTallas(estiloId);
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



