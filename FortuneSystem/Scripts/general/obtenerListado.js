﻿
$(document).ready(function () {
	var ID = $("#IdPedido").val();
	buscar_estilos(ID);
});

function probar() {
	$('#tabless tr').on('click', function (e) {
		$('#tabless tr').removeClass('highlighted');
		$(this).addClass('highlighted');
	});
	//obtener_tallas_item(id);

}

/*$(document).on("click touchend", "#tabless tr", function () {
	$('#tabless tr').removeClass('highlighted');
	$(this).addClass('highlighted');
});*/
var cliente;
$(document).on("dblclick touchend", "#tabless tr", function () {	
	var row = this.rowIndex;
	var numQtyPrint = "";
	if (row !== 0) {
		var numEstilo = $('#tabless tr:eq(' + row + ') td:eq(0)').html();
		if (cliente === "2") {
			numQtyPrint = $('#tabless tr:eq(' + row + ') td:eq(6)').html();
		} else {
			numQtyPrint = $('#tabless tr:eq(' + row + ') td:eq(5)').html();
		}
		//var estilo = $('#tabless tr:eq(' + row + ') td:eq(2)').html();
		obtener_tallas_item(numEstilo, numQtyPrint);
	}
	
	
});


$(document).on("input", ".numeric", function () {
	this.value = this.value.replace(/\D/g, '');
});

$(document).on("input", ".number", function () {
	this.value = this.value.replace(/\D/g, '');
});

//Registrar Batch

function registrarBatch() {
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
	$('#tablePrint').find('td.printed').each(function (i, el) {
		var valor = $(el).children().val();
		if (valor === '') {
			error++;
			$(el).children().css('border', '2px solid #e03f3f');

		} else {
			$(el).children().css('border', '');
		}
	});

	$('#tablePrint').find('td.cMisP').each(function (i, el) {
		var valor = $(el).children().val();
		if (valor === '') {
			error++;
			$(el).children().css('border', '2px solid #e03f3f');

		} else {
			$(el).children().css('border', '');
		}
	});

	$('#tablePrint').find('td.cDeft').each(function (i, el) {
		var valor = $(el).children().val();
		if (valor === '') {
			error++;
			$(el).children().css('border', '2px solid #e03f3f');

		} else {
			$(el).children().css('border', '');
		}
	});

	$('#tablePrint').find('td.cRepa').each(function (i, el) {
		var valor = $(el).children().val();
		if (valor === '') {
			error++;
			$(el).children().css('border', '2px solid #e03f3f');

		} else {
			$(el).children().css('border', '');
		}
	});

	var turno = $('#PrintShopC_Turnos option:selected').val();
	if (turno === "0") {
		error++;
		$('#PrintShopC_Turnos').css('border', '2px solid #e03f3f');
	}
	else {
		$('#PrintShopC_Turnos').css('border', '');
	}

	var maquina = $("#PrintShopC_Maquinas option:selected").val();
	if (maquina === "0") {
		error++;
		$('#PrintShopC_Maquinas').css('border', '2px solid #e03f3f');
	}
	else {
		$('#PrintShopC_Maquinas').css('border', '');
	}
	enviarListaTallaPrintShop(cadena, error);
}

var estiloId;
function enviarListaTallaPrintShop(cadena, error) {
	var idTurno = $("#PrintShopC_Turnos option:selected").val();
	var idMaquina = $("#PrintShopC_Maquinas option:selected").val();
	var idStatus = $("input[name='PrintShopC.EstadoPallet']:checked").val();
	var comentario = $("#PrintShopC_Comentarios").val();
	if (error !== 0) {
		var alert = alertify.alert("Message", 'All fields are required.').set('label', 'Aceptar');
		alert.set({ transition: 'zoom' });
		alert.set('modal', false);
	} else {
		$.ajax({
			url: "/PrintShop/Obtener_Lista_Tallas_PrintShop",
			datatType: 'json',
			data: JSON.stringify({ ListTalla: cadena, TurnoID: idTurno, EstiloID: estiloId, MaquinaID: idMaquina, StatusID: idStatus, Comentarios: comentario }),
			cache: false,
			type: 'POST',
			contentType: 'application/json',
			success: function (data) {
				alertify.set('notifier', 'position', 'top-right');
				alertify.notify('The batch was correctly registered.', 'success', 5, null);
				$('.number').val('0');
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
	$('#tablePrint').find('td.printed').each(function (i, el) {

		var valor = $(el).children().val();

		if (valor === '') {
			error++;
			$(el).children().css('border', '2px solid #e03f3f');

		} else {

			$(el).children().css('border', '');
		}
	});



	var turno = $('#PrintShopC_Turnos option:selected').val();
	if (turno === "0") {
		error++;
		$('#PrintShopC_Turnos').css('border', '2px solid #e03f3f');
	}
	else {
		$('#PrintShopC_Turnos').css('border', '');
	}

	var maquina = $("#PrintShopC_Maquinas option:selected").val();
	if (maquina === "0") {
		error++;
		$('#PrintShopC_Maquinas').css('border', '2px solid #e03f3f');
	}
	else {
		$('#PrintShopC_Maquinas').css('border', '');
	}
	enviarListaTallaBatchPrintShop(cadena, error, batchID);
}

function enviarListaTallaBatchPrintShop(cadena, error, batchID) {
	var idTurno = $("#PrintShopC_Turnos option:selected").val();
	var idMaquina = $("#PrintShopC_Maquinas option:selected").val();
	var idStatus = $("input[name='PrintShopC.EstadoPallet']:checked").val();
	var comentario = $("#PrintShopC_Comentarios").val();
	if (error !== 0) {
		var alert = alertify.alert("Message", 'All fields are required.').set('label', 'Aceptar');
		alert.set({ transition: 'zoom' });
		alert.set('modal', false);
	} else {
		$.ajax({
			url: "/PrintShop/Actualizar_Lista_Tallas_Batch",
			datatType: 'json',
			data: JSON.stringify({
				ListTalla: cadena, TurnoID: idTurno, EstiloID: estiloId, IdBatch: batchID, MaquinaID: idMaquina, StatusID: idStatus, Comentarios: comentario
			}),
			cache: false,
			type: 'POST',
			contentType: 'application/json',
			success: function (data) {
				alertify.set('notifier', 'position', 'top-right');
				alertify.notify('The batch was modified correctly.', 'success', 5, null);
				$('.number').val('0');
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
				html += '<tr onclick="probar();">';
				html += '<td>' + item.IdItems + '</td>';
				cliente = item.NumCliente;
				if (item.NumCliente === "2" || item.NumCliente === 2) {
					var poF = item.POFantasy === null ? "-" : item.POFantasy;
					html += '<td>' + poF + '</td>';
				}   
				html += '<td>' + item.EstiloItem + '</td>';
				html += '<td>' + item.ItemDescripcion.Descripcion + '</td>';
				html += '<td>' + item.CatColores.CodigoColor + '</td>';
				html += '<td>' + item.CatColores.DescripcionColor + '</td>';
				html += '<td>' + item.Cantidad + '</td>';
				html += '<td>' + item.Price + '</td>';
				html += '<td>' + item.Total + '</td>';
				//  html += '<td><a href="#" onclick="obtener_tallas_item(' + item.IdItems + ');" class = "btn btn-default glyphicon glyphicon-search l1s" style = "color:black; padding:0px 5px 0px 5px;" Title = "Sizes"></a></td>';
				html += '</tr>';

			});
			if (Object.keys(lista_estilo).length === 0) {
				html += '<tr class="odd"><td valign="middle" align="center" colspan="10" class="dataTables_empty">No styles were found for the PO.</td></tr>';

			}
			$('.tbody').html(html);
			$("#div_estilos_orden").css("visibility", "visible");
			$("#div_tabla_print").hide();

			$(window).scrollTop(tempScrollTop);
		},
		error: function (errormessage) { alert(errormessage.responseText); }
	});
}
var listaPO;

function obtener_tallas_item(IdEstilo, numQtyPrint) {
	var tempScrollTop = $(window).scrollTop();
	$("#panelPrintShop").css('display', 'inline');
	$("#loading").css('display', 'inline');
	$("#InfoSummary_IdItems").val(IdEstilo);
	estiloId = IdEstilo;
	obtener_tallas_PO(IdEstilo);
	$.ajax({
		url: "/Pedidos/Lista_Tallas_Estilo_PrintShop/",
		data: "{'id':'" + IdEstilo + "','numQtyPrint':'" + numQtyPrint + "'}",
		type: "POST",
		contentType: "application/json;charset=UTF-8",
		dataType: "json",
		success: function (jsonData) {
			var html = '';
			var estilos = jsonData.Data.estilos;
			var EstiloDescription;
			var lista_estilo_Desc = jsonData.Data.listaTalla;
			var lista_Qty_Tallas = jsonData.Data.listTallaCant;
			$.each(lista_estilo_Desc, function (key, item) {

				EstiloDescription = item.DescripcionEstilo;

			});
			var lista_Datos_Staging = jsonData.Data.listDatosStaging;
			var datoColor = "";
			var datoPais = "";
			var datoPorc = "";
			$.each(lista_Datos_Staging, function (key, item) {

				datoColor = item.NombreColor;
				datoPais = item.Pais;
				datoPorc = item.Porcentaje;

			});
			var datoColor2 = datoColor === "" ? "N/A" : datoColor;
			var datoPais2 = datoPais === "" ? "N/A" : datoPais;
			var datoPorc2 = datoPorc === "" ? "N/A" : datoPorc;

			var dt = $("#InfoSummary_IdItems").val();
			if (estilos !== '') {
				$("#div_datos_staging").html("<h3>Color:" + $.trim(datoColor2) + " ---" + " País:" + $.trim(datoPais2) + " --- %:" + $.trim(datoPorc2) + "</h3>");
				$("#div_estilo").html("<h2>Item: " + estilos + "-" + $.trim(EstiloDescription) + "</h2>");
				$("#div_estilo").show();
				$("#div_datos_staging").show();
			} else {
				$("#div_estilo").hide();
				$("#div_datos_staging").hide();
			}

			var lista_estilo = jsonData.Data.listaTalla;
			listaEstiloPO = lista_estilo;
			html += '<tr> <th>  </th>';
			$.each(lista_estilo, function (key, item) {

				html += '<th>' + item.Talla + '</th>';

			});
			html += '<th> Total </th>';
			html += '</tr><tr><td>Total Orden</td>';
			var cantidadesPOTotal = 0;
			var cadena_cantidadesTotal = "";
			$.each(lista_estilo, function (key, item) {

				html += '<td class="total" >' + item.Cantidad + '</td>';
				cantidadesPOTotal += item.Cantidad;
				cadena_cantidadesTotal += "*" + item.Cantidad;
			});
			var cantidades_arrayTotal = cadena_cantidadesTotal.split('*');
			html += '<td>' + cantidadesPOTotal + '</td>';
			html += '</tr>';
			html += '</tr><tr><td>1rst Quantity</td>';
			var cantidadesPO = 0;
			var cadena_cantidades = "";
			$.each(lista_Qty_Tallas, function (key, item) {

				html += '<td class="total" >' + item.Cantidad + '</td>';
				cantidadesPO += item.Cantidad;
				cadena_cantidades += "*" + item.Cantidad;
			});
			var cantidades_array = cadena_cantidades.split('*');
			html += '<td>' + cantidadesPO + '</td>';
			html += '</tr>';
			var numTallas = 0;
			$.each(lista_estilo, function (key, item) {
				numTallas++;
			});
			html += '</tr><tr><td>Staging Quantity</td>';
			var cantidades = 0;
			var lista_Staging = jsonData.Data.listTallaStaging;//listaStaging.length;
			if (lista_Staging.length === 0) {
				var total = 0;
				for (var v = 0; v < numTallas; v++) {

					html += '<td>' + total + '</td>';
					cantidades += total;
				}

			}
			$.each(lista_Staging, function (key, item) {
				html += '<td>' + item.total + '</td>';

				cantidades += item.total;
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
				lista_Batch;
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
				lista_Batch_Printed;
			}
			$.each(lista_Batch_Printed, function (key, item) {
				if (listaPBatch === 0) {
					item = 0;
					html += '<td>' + item + '</td>';
				} else {
					html += '<td>' + item + '</td>';
				}

				// cantidadesPrinted += item;
			});
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
				lista_Batch_MP;
			}
			$.each(lista_Batch_MP, function (key, item) {
				if (listaMPBatch === 0) {
					item = 0;
					html += '<td>' + item + '</td>';
				} else {
					html += '<td>' + item + '</td>';
				}

				// cantidadesMisPrintB += item;
			});
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
				lista_Batch_Defect;
			}
			$.each(lista_Batch_Defect, function (key, item) {
				if (listaDefBatch === 0) {
					item = 0;
					html += '<td>' + item + '</td>';
				} else {
					html += '<td>' + item + '</td>';
				}

				// cantidadesDefectB += item;
			});
			html += '</tr>';
			html += '</tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+ Repair</td>';
			var cantidadesRepairB = 0;
			var lista_Batch_Repair = jsonData.Data.listaTallasTotalRBatch;
			var listaRepBatch = 0;
			$.each(lista_Batch_Repair, function (key, item) {
				listaRepBatch++;
			});
			if (listaRepBatch === 0) {
				lista_Batch_Repair = lista_estilo;
			} else {
				lista_Batch_Repair;
			}
			$.each(lista_Batch_Repair, function (key, item) {
				if (listaRepBatch === 0) {
					item = 0;
					html += '<td>' + item + '</td>';
				} else {
					html += '<td>' + item + '</td>';
				}

				// cantidadesRepairB += item;
			});
			html += '</tr>';
			html += '<tr><td>+/-</td>';
			var totales = 0;
			var i = 1;
			var sumaTotal = 0;
			$.each(lista_Batch, function (key, item) {
				if (listaTBatch === 0) {
					item = 0;
				}
				var resta = parseFloat(item) - parseFloat(cantidades_arrayTotal[i]);

				if (resta === 0) {
					html += '<td class="restaPrint" style="color:black;">' + resta + '</td>';
				} else if (resta >= 0) {
					html += '<td class="restaPrint" style="color:blue;">' + resta + '</td>';
				} else {
					html += '<td class="restaPrint" style="color:red;">' + resta + '</td>';
				}
				i++;
				sumaTotal += resta;
			});
			html += '<td>' + sumaTotal + '</td>';

			html += '</tr>';

			if (Object.keys(lista_estilo).length === 0) {
				html += '<tr class="odd"><td valign="middle" align="center" colspan="10" class="dataTables_empty">No sizes were found for the style.</td></tr>';
			}
			$('.tbodys').html(html);
			$("#consultaTalla").css("visibility", "visible");
			$("#div_estilo").css("visibility", "visible");
			$("#div_datos_staging").css("visibility", "visible");
			$("#arte").css("visibility", "visible");
			obtenerImagenPNL(estilos, dt);
			obtenerImagenArte(estilos);
			obtener_bacth_estilo(IdEstilo);
			if (sumaTotal !== 0) {
				obtenerTallas_PrintShop(IdEstilo);
			}

			//obtenerIdEstilo(IdEstilo);
			$("#loading").css('display', 'none');
			$(window).scrollTop(tempScrollTop);
		},
		error: function (errormessage) { alert(errormessage.responseText); }
	});
}

$(document).on('click', '#registrarNuevo', function () {
	obtenerTallas_PrintShop(estiloId);
});
var size;
function obtener_bacth_estilo(IdEstilo) {
	var tempScrollTop = $(window).scrollTop();
	//  $("#loading").css('display', 'inline');
	$.ajax({
		url: "/PrintShop/Lista_Batch_Estilo/" + IdEstilo,
		type: "POST",
		contentType: "application/json;charset=UTF-8",
		dataType: "json",
		success: function (jsonData) {
			var html = '';

			var estilos = jsonData.Data.estilos;
			var cargoUser = jsonData.Data.cargoUser;
			var sucursal = jsonData.Data.sucursal;
			if (estilos !== '') {
				$("#div_estilo_batch").html("<h2>BATCH REVIEW <h4> STATUS (C-complete / I-incomplete)</h4> </h2> ");
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
			html += '<th> Comments </th>';
			html += '<th> Shift </th>';
			html += '<th> Machine </th>';
			html += '<th> User Modif </th>';
			html += '<th> Status </th>';
			html += '<th> Actions </th>';
			html += '</tr>';


			$.each(lista_batch, function (key, item) {
				html += '<tr><td>Pallet-' + item.IdBatch + '</td>';

				var cantidad = 0;
				$.each(item.Batch, function (key, i) {

					html += '<td class="total" >' + i.Total + '</td>';
					cantidad += i.Total;
				});
				html += '<td>' + cantidad + '</td>';
				html += '<td>' + item.NombreUsr + '</td>';
				if (item.Comentarios === '') {
					item.Comentarios = 'N/A';
					html += '<td>' + item.Comentarios + '</td>';
				} else {
					html += '<td>' + item.Comentarios + '</td>';
				}

				if (sucursal === "FORTUNE") {
					if (item.TipoTurno === 1) {
						html += '<td>First Turn</td>';
					} else {
						html += '<td>Second Turn</td>';
					}
				} else if (sucursal === "LUCKY1") {
					if (item.TipoTurno === 1) {
						html += '<td>First Turn - Lucky1</td>';
					} else {
						html += '<td>Second Turn - Lucky1</td>';
					}
					//	html += '<td>Lucky1</td>';
				}

				html += '<td>' + item.NombreMaquina + '</td>';
				html += '<td>' + item.NombreUsrModif + '</td>';
				html += '<td>' + item.Status + '</td>';
				if (cargoUser === 5 || cargoUser === 1) {
					html += '<td><a href="#" onclick="obtenerTallas_Batch(' + item.IdBatch + ',' + item.TipoTurno + ',' + item.Maquina + ',' + item.IdPrintShop + ',\'' + item.Comentarios + '\'' + ',\'' + item.Status + '\');" class = "btn btn-default glyphicon glyphicon-search l1s" style = "color:black; padding:0px 5px 0px 5px;" Title = "Details Bacth"></a>';
					html += '<a href="#" onclick="event.preventDefault();ConfirmDeleteBatch(' + item.IdBatch + ',' + IdEstilo + ')" class = "btn btn-default glyphicon glyphicon-trash l1s" style = "color:black; padding:0px 5px 0px 5px;" Title = "Delete Bacth"></a></td>';
				} else {
					html += '<td></td>';
				}

				html += '</tr>';


			});
			if (Object.keys(lista_batch).length === 0) {
				html += '<tr class="odd"><td valign="middle" align="center" colspan="10" class="dataTables_empty">No batches were found for the style.</td></tr>';

			}
			$('.tbodyBatch').html(html);
			$("#div_estilo_batch").css("visibility", "visible");
			$("#loading").css('display', 'none');
			$(window).scrollTop(tempScrollTop);
			var IdEstiloInf = $("#InfoSummary_IdItems").val();
			//Lista de Printshop
			obtenerListaPrint(IdEstiloInf);

		},
		error: function (errormessage) { alert(errormessage.responseText); }
	});
	calcular_Restantes();
}

function obtener_tallas_PO(IdEstilo) {
	$.ajax({
		url: "/Pedidos/Listado_Tallas_Estilo_Print/" + IdEstilo,
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

function obtenerImagenPNL(nombreEstilo, numEstilo) {
	$('#imagenPNL').attr('src', '/Arte/ConvertirImagenPNLEstilo?nombreEstilo=' + nombreEstilo + '&IdItem=' + numEstilo);
	//+ '&color=' + color
}

function obtenerImagenArte(nombreEstilo) {
	$('#imagenArte').attr('src', '/Arte/BuscarImagenArte?nombreEstilo=' + nombreEstilo);
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
	$("#PrintShopC_Turnos").val(0);
	$("#PrintShopC_Maquinas").val(0);
	$("#PrintShopC_Comentarios").val('');
	//$("#loading").css('display', 'inline');
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
				$("#registrarNuevo").hide();
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
			var cadena_cantidades = "";
			$.each(listaPO, function (key, item) {

				cadena_cantidades += "*" + item.Cantidad;
			});
			var cantidades_array = cadena_cantidades.split('*');
			var lista_Batch = jsonData.Data.listaPrint;
			var listaBat = lista_Batch.length;
			if (listaBat === 0) {
				lista_Batch = listaPO;
			} else {
				lista_Batch;
			}
			var totales = 0;
			var i = 1;
			$.each(lista_Batch, function (key, item) {
				if (listaBat === 0) {
					html += '<td id="po"><input type="text" id="po" class="txtDes form-control cantPO"  value="' + item.Cantidad + '"/></td>';
				} else {
					var resta = parseFloat(cantidades_array[i]) - parseFloat(item);
					html += '<td id="po"><input type="text" id="po" class="txtDes form-control cantPO"  value="' + resta + '"/></td>';
					i++;
				}

				cantidadesPO += item.Cantidad;
			});

			html += '<th> QTY </th>';
			html += '<tr > <th>  </th>';
			//*************************************************
			var lista_estilo_Tallas = jsonData.Data.listaEstiloTallas;
			$.each(lista_estilo_Tallas, function (key, item) {

				html += '<td><input type="text" id="talla" class="txtDes form-control talla" value="' + item.Talla + '"/></td>';

			});

			html += '<th> Total </th>';
			html += '</tr><tr><td>Printed</td>';
			var cantidades = 0;
			var contadorQty = 0;
			$.each(lista_estilo_Tallas, function (key, item) {
				item.Printed = item.Talla;
				item.Printed = 0;
				html += '<td class="printed"><input type="text" id="cantidad' + contadorQty+'" class="txt form-control print numeric" onfocus="focusing(' + contadorQty +')" onChange="calcular_Printed()" value="' + item.Printed + '"/></td>';
				cantidades += item.Printed;
				contadorQty++;
			});
			html += '<td><input type="text" id="totalP" class="form-control number"  value="' + cantidades + '" readonly/></td>';
			html += '</tr><tr><td>MisPrint</td>';
			var misPrintCant = 0;
			var contadorMP = 0;
			$.each(lista_estilo_Tallas, function (key, item) {

				item.MisPrint = item.Talla;
				item.MisPrint = 0;
				html += '<td class="cMisP"><input type="text" id="misprint' + contadorMP+'" class="txt form-control mp numeric"  onfocus="focusingMP(' + contadorMP +')" onChange="calcular_MisPrint()" value="' + item.MisPrint + '"/></td>';
				misPrintCant += item.MisPrint;
				contadorMP++;
			});
			html += '<td><input type="text" id="totalM" class="form-control number totalM" value="' + misPrintCant + '" readonly/></td>';
			html += '</tr><tr ><td class="dato">Defect</td>';
			var defectCant = 0;
			var contadorD = 0;
			$.each(lista_estilo_Tallas, function (key, item) {

				item.Defect = item.Talla;
				item.Defect = 0;
				html += '<td class="cDeft"><input type="text" id="defect' + contadorD+'" class="txt form-control def numeric " onfocus="focusingD(' + contadorD +')" onChange="calcular_Defect()" value="' + item.Defect + '"/></td>';
				defectCant += item.Defect;
				contadorD++;
			});
			html += '<td><input type="text" id="totalD" class="form-control number totalD" value="' + defectCant + '" readonly/></td>';
			html += '</tr><tr ><td class="dato">Repair</td>';
			var repairCant = 0;
			var contadorR = 0;
			$.each(lista_estilo_Tallas, function (key, item) {

				item.Repair = item.Talla;
				item.Repair = 0;
				html += '<td class="cRepa"><input type="text" id="repair' + contadorR+'" class="txt form-control rep numeric " onfocus="focusingR(' + contadorR +')" onChange="calcular_Repair()" value="' + item.Repair + '"/></td>';
				repairCant += item.Repair;
				contadorR++;
			});
			html += '<td><input type="text" id="totalR" class="form-control number totalR" value="' + repairCant + '" readonly/></td>';
			html += '</tr><tr ><td class="total">+/-</td>';
			var total = 0;
			$.each(lista_estilo_Tallas, function (key, item) {
				html += ' <div class="span7">';
				item.Defect = item.Talla;
				item.Defect = 0;
				html += '<td ><input type="text" id="falt" class="form-control number totalFal" value="' + item.Defect + '" readonly/></td>';

				html += ' </div>';
				total = cantidades + misPrintCant + defectCant + repairCant;
			});
			html += '<td><input type="text" id="totalF" class="form-control number totalF" value="' + total + '" readonly/></td>';
			html += '</tr>';


			$('.tbodyprint').html(html);
			$("#div_estilo_print").css("visibility", "visible");
			$("#div_tabla_print").show();
			// $("#loading").css('display', 'none');
			$(window).scrollTop(tempScrollTop);

		},
		error: function (errormessage) { alert(errormessage.responseText); }
	});
}
function ConfirmDeleteBatch(idBatch, idEstilo) {
	alertify.confirm("Are you sure you want to delete pallet ?", function (result) {
		$.ajax({
			url: '/PrintShop/EliminarBatch/',
			data: "{'idBatch':'" + idBatch + "', 'idEstilo':'" + idEstilo + "'}",
			dataType: 'json',
			contentType: 'application/json',
			type: 'post',
			success: function () {
				obtener_tallas_item(idEstilo);
			}
		});
	});
}

function focusing(valor) {
	if ($("#cantidad" + valor).val() === 0 || $("#cantidad" + valor).val() === "0") {
		$("#cantidad" + valor).val('');
	}
}

function focusingMP(valor) {
	if ($("#misprint" + valor).val() === 0 || $("#misprint" + valor).val() === "0") {
		$("#misprint" + valor).val('');
	}
}

function focusingD(valor) {
	if ($("#defect" + valor).val() === 0 || $("#defect" + valor).val() === "0") {
		$("#defect" + valor).val('');
	}
}

function focusingR(valor) {
	if ($("#repair" + valor).val() === 0 || $("#repair" + valor).val() === "0") {
		$("#repair" + valor).val('');
	}
}


function obtenerTallas_Batch(idBatch, idTurno, idMaquina, idPrintShop, comentario, idStatus) {
	var tempScrollTop = $(window).scrollTop();
	$("#div_tabla_print").show();
	$("#PrintShopC_Turnos").val(idTurno);
	$("#PrintShopC_Maquinas").val(idMaquina);
	//$("#PrintShopC_Comentarios").val(comentario);
	if (idStatus === "C") {
		$("input[name='PrintShopC.EstadoPallet'][value='true']").prop("checked", true);
	} else {
		$("input[name='PrintShopC.EstadoPallet'][value='false']").prop("checked", true);
	}

	$('#PrintShopC_Turnos').css('border', '');

	$('#PrintShopC_Maquinas').css('border', '');

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
				$("#registrarNuevo").show();
			} else {
				$("#div_estilo_print").hide();
			}

			var lista_estilo = jsonData.Data.listaTalla;
			var list = lista_estilo.length;
			if (lista_estilo === 0) {
				lista_estilo = listaEstiloPO;
			} else {
				lista_estilo;

			}
			html += '</tr><tr><td>PO </td>';
			var cantidadesPO = 0;
			var cadena_cantidades = "";
			$.each(listaPO, function (key, item) {

				cadena_cantidades += "*" + item.Cantidad;
			});
			var cantidades_array = cadena_cantidades.split('*');
			var lista_Batch = jsonData.Data.listaPrint;
			var listaBat = lista_Batch.length;
			if (listaBat === 0) {
				lista_Batch = listaPO;
			} else {
				lista_Batch;
			}
			var totales = 0;
			var i = 1;
			$.each(lista_Batch, function (key, item) {
				if (listaBat === 0) {
					html += '<td id="po"><input type="text" id="po" class="txtDes form-control cantPO"  value="' + item.Cantidad + '"/></td>';
				} else {
					var resta = parseFloat(cantidades_array[i]) - parseFloat(item);
					html += '<td id="po"><input type="text" id="po" class="txtDes form-control cantPO"  value="' + resta + '"/></td>';
					i++;
				}

				cantidadesPO += item.Cantidad;
			});
			html += '<th> QTY </th>';
			html += '<tr > <th>  </th>';
			$.each(lista_estilo, function (key, item) {
				html += '<td><input type="text" id="talla" class="txtDes form-control talla" value="' + item.Talla + '"/></td>';

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
					html += '<td><input type="text" id="cantidad" class="txt form-control print numeric" onChange="calcular_Printed()" value="' + item.Printed + '"/></td>';
				} else {

					html += '<td><input type="text" id="cantidad" class="txt form-control print numeric"  onChange="calcular_Printed()" value="' + item.Printed + '"/></td>';
				}

				cantidades += item.Printed;
			});
			html += '<td><input type="text" id="totalP" class="form-control number"  value="' + cantidades + '" readonly/></td>';
			html += '</tr><tr><td>MisPrint</td>';
			var misPrintCant = 0;
			$.each(lista_estilo, function (key, item) {
				if (list === 0) {
					item.MisPrint = 0;
					html += '<td ><input type="text" id="misprint" class="txt form-control mp numeric" onChange="calcular_MisPrint()" value="' + item.MisPrint + '"/></td>';
				} else {

					html += '<td > <input type="text" id="misprint" class=" txt form-control mp numeric" onChange="calcular_MisPrint()" value="' + item.MisPrint + '"/></td>';
				}

				misPrintCant += item.MisPrint;
			});
			html += '<td><input type="text" id="totalM" class="form-control number totalM" value="' + misPrintCant + '" readonly/></td>';
			html += '</tr><tr ><td class="dato">Defect</td>';
			var defectCant = 0;
			$.each(lista_estilo, function (key, item) {
				if (list === 0) {
					item.Defect = 0;
					html += '<td ><input type="text" id="defect" class="txt form-control def numeric " onChange="calcular_Defect()" value="' + item.Defect + '"/></td>';
				} else {

					html += '<td ><input type="text" id="defect" class="txt form-control def numeric" onChange="calcular_Defect()" value="' + item.Defect + '"/></td>';
				}

				defectCant += item.Defect;
			});
			html += '<td><input type="text" id="totalD" class="form-control number totalD" value="' + defectCant + '" readonly/></td>';
			html += '</tr><tr ><td class="dato">Repair</td>';
			var repairCant = 0;
			$.each(lista_estilo, function (key, item) {
				if (list === 0) {
					item.Repair = 0;
					html += '<td ><input type="text" id="repair" class="txt form-control rep numeric " onChange="calcular_Repair()" value="' + item.Repair + '"/></td>';
				} else {

					html += '<td ><input type="text" id="repair" class="txt form-control rep numeric" onChange="calcular_Repair()" value="' + item.Repair + '"/></td>';
				}

				repairCant += item.Repair;
			});
			html += '<td><input type="text" id="totalR" class="form-control number totalR" value="' + repairCant + '" readonly/></td>';
			/* html += '</tr><tr ><td class="total">+/-</td>';
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
				 total = cantidades + misPrintCant + defectCant + repairCant;
			 });*/
			html += '</tr><tr ><td class="total">+/-</td>';
			var total = 0;
			$.each(lista_estilo, function (key, item) {
				html += ' <div class="span7">';
				item.Defect = item.Talla;
				item.Defect = 0;
				html += '<td ><input type="text" id="falt" class="form-control number totalFal" value="' + item.Defect + '" readonly/></td>';

				html += ' </div>';
				total = cantidades + misPrintCant + defectCant + repairCant;
			});
			html += '<td><input type="text" id="totalF" class="form-control number totalF" value="' + total + '" readonly/></td>';
			html += '</tr>';

			$('.tbodyprint').html(html);
			$("#div_estilo_print").css("visibility", "visible");
			CalcularTotalBatch();
			CalcularTotal();
			calcular_Restantes();
			$(window).scrollTop(tempScrollTop);
		},
		error: function (errormessage) { alert(errormessage.responseText); }
	});
}

