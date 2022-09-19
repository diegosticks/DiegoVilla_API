using DiegoVilla_VillaAPI.Data;
using DiegoVilla_VillaAPI.Logging;
using DiegoVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace DiegoVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        //private readonly ILogger<VillaAPIController> _logger;

        ////This is for logging files and creates a folder which i have deleted. Serilog dependency needed too
        //public VillaAPIController(ILogger<VillaAPIController> logger)
        //{
        //    _logger = logger;
        //}

        //private readonly ILogging _logger;
        //// This is for custom logger
        //public VillaAPIController(ILogging logger)
        //{
        //    _logger = logger;
        //}


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult <IEnumerable<VillaDTO>> GetVillas()
        {
            //_logger.Log("Getting Villas..", "");
            return Ok(VillaStore.VillaList);
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult <VillaDTO> GetVilla(int id)
        {
            if(id == 0)
            {
                //_logger.Log("Get Villa error with Id =" + id, "error");
                return BadRequest();
            }

            var villa = VillaStore.VillaList.FirstOrDefault(u => u.Id == id);
            if(villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            // if you don't want to use the [APIController], this will validate the model
            //if (ModelState.IsValid) { return BadRequest(ModelState); }
            if(VillaStore.VillaList.FirstOrDefault(u=>u.Name.ToLower()==villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustoomeError", "Villa Already Exists!");
                return BadRequest(ModelState);
            }
            if(villaDTO == null)
            {
                return BadRequest(villaDTO);
            }
            if(villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villaDTO.Id = VillaStore.VillaList.OrderByDescending(u=>u.Id).FirstOrDefault().Id + 1;
            VillaStore.VillaList.Add(villaDTO);

            return CreatedAtRoute("GetVilla", new {Id = villaDTO.Id}, villaDTO);
        }

        [HttpDelete("{Id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int Id)
        {
            if (Id == 0)
            {
                return BadRequest();
            }

            var villa = VillaStore.VillaList.FirstOrDefault(u=>u.Id==Id);
            if( villa == null)
            {
                return NotFound();
            }
            VillaStore.VillaList.Remove(villa);

            return NoContent();
        }

        [HttpPut("{Id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int Id, [FromBody]VillaDTO villaDTO)
        {
            if(villaDTO == null || Id != villaDTO.Id)
            {
                return BadRequest();
            }

            var villa = VillaStore.VillaList.FirstOrDefault(u=>u.Id == Id);
            villa.Name = villaDTO.Name;
            villa.Occupancy = villaDTO.Occupancy;
            villa.Sqft = villaDTO.Sqft;

            return NoContent();

        }

        [HttpPatch("{Id:int}", Name = "UpdatepartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int Id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if (patchDTO == null || Id == 0)
            {
                return BadRequest();
            }

            var villa = VillaStore.VillaList.FirstOrDefault(u => u.Id == Id);
            if(villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villa, ModelState);
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();

        }
    }
}
