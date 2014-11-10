﻿using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Batching.Entities;
using MrCMS.Web.Areas.Admin.Services.Batching;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate.Linq;
using Ninject;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class BatchExecutionController : MrCMSAdminController
    {
        private readonly IBatchRunUIService _batchRunUIService;

        public BatchExecutionController(IBatchRunUIService batchRunUIService)
        {
            _batchRunUIService = batchRunUIService;
        }

        public JsonResult ExecuteNext([IoCModelBinder(typeof(BatchRunGuidModelBinder))]BatchRun run)
        {
            var result = run == null ? null : _batchRunUIService.ExecuteNextTask(run);
            if (result != null)
            {
                _batchRunUIService.ExecuteRequestForNextTask(run);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }

    public class BatchRunGuidModelBinder : MrCMSDefaultModelBinder
    {
        public BatchRunGuidModelBinder(IKernel kernel)
            : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var id = Convert.ToString(controllerContext.RouteData.Values["id"]);
            Guid guid;
            if (Guid.TryParse(id, out guid))
            {
                return Session.Query<BatchRun>().FirstOrDefault(x => x.Guid == guid);
            }
            return null;
        }
    }
}