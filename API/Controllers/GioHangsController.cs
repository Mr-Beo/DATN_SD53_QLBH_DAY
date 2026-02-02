using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data.Models;
using API.IRepository;

namespace API.Controllers
{
    public class GioHangsController : Controller
    {
        private readonly IGioHangRepository _giohang;

        public GioHangsController(IGioHangRepository giohang)
        {
            _giohang = giohang;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllGioHang()
        {
            return Ok(await _giohang.GetAllGioHang());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGioHangByID(Guid id) {
            return Ok(await _giohang.GetGioHang(id));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( GioHang gioHang)
        {
            await _giohang.CreateGioHang(gioHang);
            return Ok();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( GioHang gioHang)
        {
            await _giohang.UpdateGioHang(gioHang);
            return Ok();
            
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _giohang.DeleteGioHang(id);
            return Ok();
        }

    }
}
