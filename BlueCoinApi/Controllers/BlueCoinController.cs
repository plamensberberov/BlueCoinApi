using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlueCoin.Blockchain;
using BlueCoinApi.Models;

namespace BlueCoinApi.Controllers
{
    [Route("api/coin")]
    [ApiController]
    public class BlueCoinController : ControllerBase
    {
        public Blockchain _blockchain;

        [HttpGet]
        [Route("genesis")]
        public IActionResult StartBlockchain()
        {
            try
            {
                _blockchain = new Blockchain();
                return Ok(_blockchain);
            }
            catch(Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("add-transaction")]
        public IActionResult AddTransaction([FromBody]AddTransactionViewModel model)
        {
            try
            {
                var transaction = _blockchain.AddTransaction(model.Sender, model.Receiver, model.Amount);

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("mine-first")]
        public IActionResult MineTransaction(int minerId)
        {
            try
            {
                var success = _blockchain.MineFirstPendingTransaction(minerId);
                if (success) return Ok("Block Mined");
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
